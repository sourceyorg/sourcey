using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.Keys;
using Sourcey.Projections;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Projections;
using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections.Reset;

public class When_projection_reset_and_rebuilt : EntityFrameworkIntegrationSpecification,
    IClassFixture<HostFixture>,
    IClassFixture<EntityFrameworkCoreWebApplicationFactory>
{
    private readonly string _subject = Subject.New();

    public When_projection_reset_and_rebuilt(HostFixture hostFixture, EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper output) : base(hostFixture, factory, output) { }

    protected override async Task Given()
    {
        using var scope = _factory.Services.CreateScope();
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();

        var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
        var id = StreamId.From(_subject);
        // Seed a single event so rebuild creates one record without requiring an update path
        aggregate.MakeSomethingHappen(id, "before-reset");
        await aggregateStore.SaveAsync(aggregate, default);
    }

    protected override async Task When()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IProjectionManager<Something>>();
        await manager.ResetAsync();
    }

    [Integration]
    public async Task Then_projection_rebuilds_to_latest_state()
    {
        using var scope = _factory.Services.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();

        // After ResetAsync, the projection rebuild runs on a background interval (configured to ~1s).
        // Use a 30 x 1s bounded wait window to avoid flakiness in CI.
        var projection = await reader.ReadAsync(_subject, s => s != null && s.Value == "before-reset", 30, TimeSpan.FromSeconds(1));
        projection.ShouldNotBeNull();
        projection!.Value.ShouldBe("before-reset");
    }
}
