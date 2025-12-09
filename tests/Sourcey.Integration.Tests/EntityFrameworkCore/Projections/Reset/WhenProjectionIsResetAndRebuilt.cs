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

public class WhenProjectionIsResetAndRebuilt : EntityFrameworkIntegrationSpecification,
    IClassFixture<HostFixture>,
    IClassFixture<EntityFrameworkCoreWebApplicationFactory>
{
    private readonly string _subject = Subject.New();

    public WhenProjectionIsResetAndRebuilt(HostFixture hostFixture, EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper output) : base(hostFixture, factory, output) { }

    protected override async Task Given()
    {
        using var scope = _factory.Services.CreateScope();
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();

        var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
        var id = StreamId.From(_subject);
        aggregate.MakeSomethingHappen(id, "v1");
        await aggregateStore.SaveAsync(aggregate, default);
    }

    protected override Task When() => Task.CompletedTask;

    [Integration]
    public async Task Then_state_is_rebuilt_to_latest_event()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IProjectionManager<Something>>();
        var reader  = scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();

        await manager.ResetAsync();

        // Assert that state is rebuilt (non-null) and matches the first event value.
        var projection = await reader.ReadAsync(_subject, s => s != null && s.Value == "v1", 30, TimeSpan.FromSeconds(1));
        projection.ShouldNotBeNull();
        projection!.Value.ShouldBe("v1");
    }
}
