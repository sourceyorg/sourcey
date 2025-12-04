using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.Keys;
using Sourcey.Projections;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Projections;
using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections.Incremental;

public class When_new_events_written : EntityFrameworkIntegrationSpecification,
    IClassFixture<HostFixture>,
    IClassFixture<EntityFrameworkCoreWebApplicationFactory>
{
    private readonly string _subject = Subject.New();

    public When_new_events_written(HostFixture hostFixture, EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper output) : base(hostFixture, factory, output) { }

    protected override async Task Given()
    {
        using var scope = _factory.Services.CreateScope();
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();

        var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
        aggregate.MakeSomethingHappen(StreamId.From(_subject), "first");
        await aggregateStore.SaveAsync(aggregate, default);
    }

    protected override async Task When()
    {
        using var scope = _factory.Services.CreateScope();
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();

        var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
        aggregate.MakeSomethingHappen(StreamId.From(_subject), "second");
        await aggregateStore.SaveAsync(aggregate, default);
    }

    [Integration]
    public async Task Then_projection_catches_up_with_latest_value()
    {
        using var scope = _factory.Services.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();

        var result = await reader.ReadAsync(_subject, s => s != null && s.Value == "second", 20, TimeSpan.FromMilliseconds(50));
        result.ShouldNotBeNull();
        result!.Value.ShouldBe("second");
    }
}
