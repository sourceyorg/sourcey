using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sourcey.Integration.Host.Extensions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore;

public class HostFixture : IAsyncLifetime
{
    public PostgresServerResource? EventStore { get; private set; }
    public PostgresServerResource? Projections { get; private set; }
    
    public string? EventStoreConnectionString { get; private set; }
    
    public string? ProjectionsConnectionString { get; private set; }

    private DistributedApplication? App { get; set; }

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Sourcey_Integration_Host>().ConfigureAwait(false);

        EventStore = (PostgresServerResource)appHost.Resources
            .Single(static r => r.Name == DistributedApplicationKeys.EventStore);

        Projections = (PostgresServerResource)appHost.Resources
            .Single(static r => r.Name == DistributedApplicationKeys.Projections);

        App = await appHost.BuildAsync().ConfigureAwait(false);

        var resourceNotificationService = App.Services
            .GetRequiredService<ResourceNotificationService>();

        await App.StartAsync().ConfigureAwait(false);
        

        await resourceNotificationService.WaitForResourceHealthyAsync(DistributedApplicationKeys.Projections).ConfigureAwait(false);
        await resourceNotificationService.WaitForResourceHealthyAsync(DistributedApplicationKeys.EventStore).ConfigureAwait(false);

        EventStoreConnectionString = await EventStore.GetConnectionStringAsync().ConfigureAwait(false);
        ProjectionsConnectionString = await Projections.GetConnectionStringAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        if (App is null)
            return;

        await App.StopAsync().ConfigureAwait(false);
        await App.DisposeAsync().ConfigureAwait(false);
    }
}
