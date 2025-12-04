using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.EntityFrameworkCore.Events.Stores;
using Sourcey.Events;
using Sourcey.Events.Stores;
using Sourcey.Keys;
using Sourcey.Testing.Integration.Stubs.Events;
using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Events.Store;

public class When_events_appended_and_read_back : EntityFrameworkIntegrationSpecification,
    IClassFixture<HostFixture>,
    IClassFixture<EntityFrameworkCoreWebApplicationFactory>
{
    private StreamId _streamA;
    private StreamId _streamB;

    public When_events_appended_and_read_back(HostFixture hostFixture,
        EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper output) : base(hostFixture, factory, output)
    {
    }

    protected override async Task Given()
    {
        _streamA = StreamId.New();
        _streamB = StreamId.New();

        using var scope = _factory.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IEventStore<EventStoreDbContext>>();

        // Build a few deterministic events per stream
        var aEvents = new List<IEventContext<IEvent>>
        {
            new EventContext<IEvent>(_streamA, new SomethingHappened(_streamA, null, "A1"), null, null, DateTimeOffset.UtcNow, Actor.Unknown),
            new EventContext<IEvent>(_streamA, new SomethingHappened(_streamA, null, "A2"), null, null, DateTimeOffset.UtcNow.AddMilliseconds(1), Actor.Unknown),
            new EventContext<IEvent>(_streamA, new SomethingHappened(_streamA, null, "A3"), null, null, DateTimeOffset.UtcNow.AddMilliseconds(2), Actor.Unknown)
        };

        var bEvents = new List<IEventContext<IEvent>>
        {
            new EventContext<IEvent>(_streamB, new SomethingHappened(_streamB, null, "B1"), null, null, DateTimeOffset.UtcNow, Actor.Unknown),
            new EventContext<IEvent>(_streamB, new SomethingHappened(_streamB, null, "B2"), null, null, DateTimeOffset.UtcNow.AddMilliseconds(1), Actor.Unknown)
        };

        await store.SaveAsync(_streamA, aEvents, default);
        await store.SaveAsync(_streamB, bEvents, default);
    }

    protected override Task When()
    {
        // No additional action; reads will be asserted in Then_ methods
        return Task.CompletedTask;
    }

    [Integration]
    public async Task Then_per_stream_reads_are_in_order_and_paged()
    {
        using var scope = _factory.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IEventStore<EventStoreDbContext>>();

        var a = (await store.GetEventsAsync(_streamA, pageSize: 100, default)).Select(e => ((SomethingHappened)e.Payload).Something).ToArray();
        var b = (await store.GetEventsAsync(_streamB, pageSize: 100, default)).Select(e => ((SomethingHappened)e.Payload).Something).ToArray();

        a.ShouldBe(new[] { "A1", "A2", "A3" });
        b.ShouldBe(new[] { "B1", "B2" });
    }

    [Integration]
    public async Task Then_global_paging_advances_offset()
    {
        using var scope = _factory.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IEventStore<EventStoreDbContext>>();

        var page1 = await store.GetEventsAsync(0, pageSize: 2, default);
        page1.PreviousOffset.ShouldBe(0);
        page1.Offset.ShouldBeGreaterThan(0);
        page1.Events.Sum(kvp => kvp.Value.Count()).ShouldBe(2);

        var page2 = await store.GetEventsAsync(page1.Offset, pageSize: 3, default);
        page2.PreviousOffset.ShouldBe(page1.Offset);
        page2.Events.Sum(kvp => kvp.Value.Count()).ShouldBe(3);
    }
}
