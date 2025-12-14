using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.Events.Stores;
using Sourcey.Keys;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Aggregates.Concurrency;

public class WhenExpectedVersionConflicts : EntityFrameworkIntegrationSpecification,
    IClassFixture<HostFixture>,
    IClassFixture<EntityFrameworkCoreWebApplicationFactory>
{
    private StreamId _streamId;

    public WhenExpectedVersionConflicts(HostFixture hostFixture,
        EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper output) : base(hostFixture, factory, output) { }

    protected override async Task Given()
    {
        _streamId = StreamId.New();

        using var scope = _factory.Services.CreateScope();
        var aggregates = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var store = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();

        var agg = aggregates.Create<SampleAggregate, SampleState>();
        agg.MakeSomethingHappen(_streamId, "first");
        await store.SaveAsync(agg, expectedVersion: 0, cancellationToken: default);
    }

    protected override async Task When()
    {
        using var scope = _factory.Services.CreateScope();
        var aggStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();
        var aggFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();

        // Create a new aggregate with a new event for the same stream
        var aggregate = aggFactory.Create<SampleAggregate, SampleState>();
        aggregate.MakeSomethingHappen(_streamId, "second");

        // Use an obviously incorrect expectedVersion to trigger conflict resolution BEFORE any save attempt
        await aggStore.SaveAsync(aggregate, expectedVersion: 999, cancellationToken: default);
    }

    [Integration]
    public async Task Then_no_extra_events_are_saved_when_expected_version_conflicts()
    {
        using var scope = _factory.Services.CreateScope();
        var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore<EventStoreDbContext>>();

        // Only the first event should be present
        var events = await eventStore.GetEventsAsync(_streamId, pageSize: 100, default);
        events.Count().ShouldBe(1);
    }
}
