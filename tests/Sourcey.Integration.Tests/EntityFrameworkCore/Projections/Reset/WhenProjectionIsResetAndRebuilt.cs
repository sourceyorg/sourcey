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

        var aggregate2 = aggregateFactory.Create<SampleAggregate, SampleState>();
        aggregate2.MakeSomethingHappen(id, "v2");
        await aggregateStore.SaveAsync(aggregate2, default);
    }

    protected override Task When() => Task.CompletedTask;

    [Integration]
    public async Task Then_state_is_rebuilt_to_latest_event()
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IProjectionManager<Something>>();
        var reader  = scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();

        await manager.ResetAsync();

        // Phase 1: wait until any projection exists for the subject (first event applied)
        var existing = await reader.ReadAsync(_subject, s => s != null, 60, TimeSpan.FromSeconds(1));
        existing.ShouldNotBeNull();

        // Phase 2: wait until the latest value (second event) is observed
        var result = await reader.ReadAsync(_subject, s => s != null && s.Value == "v2", 60, TimeSpan.FromSeconds(1));
        result.ShouldNotBeNull();
        result!.Value.ShouldBe("v2");
    }
}
