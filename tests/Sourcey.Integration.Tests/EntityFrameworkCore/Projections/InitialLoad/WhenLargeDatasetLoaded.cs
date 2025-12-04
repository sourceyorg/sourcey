using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.Keys;
using Sourcey.Projections;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Projections;
using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections.InitialLoad;

public class WhenLargeDatasetLoaded : EntityFrameworkIntegrationSpecification,
    IClassFixture<HostFixture>,
    IClassFixture<EntityFrameworkCoreWebApplicationFactory>
{
    private const int Count = 10_000;

    public WhenLargeDatasetLoaded(HostFixture hostFixture,
        EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper)
        : base(hostFixture, factory, testOutputHelper)
    {
    }

    protected override Task Given()
    {
        return Task.CompletedTask;
    }

    protected override async Task When()
    {
        foreach (var chunk in Enumerable.Range(0, Count).Chunk(10))
        {
            await Task.WhenAll(chunk.Select(async _ =>
            {
                await using var scope = _factory.Services.CreateAsyncScope();
                var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
                var aggregateStore = scope.ServiceProvider
                    .GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();

                var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
                aggregate.MakeSomethingHappen(StreamId.New(), "Something");
                await aggregateStore.SaveAsync(aggregate, default).ConfigureAwait(false);
            })).ConfigureAwait(false);
        }
    }

    [Integration]
    public async Task Projections_Should_BeLoadedWithin10Seconds()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var projectionManager = scope.ServiceProvider.GetRequiredService<IProjectionManager<Something>>();
        var projectionReader = scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();
        await projectionManager.ResetAsync();
        var query = await projectionReader.QueryAsync(async q => (await q.CountAsync().ConfigureAwait(false)) == Count, 5,
            TimeSpan.FromSeconds(2));
        var count = await query.CountAsync();

        count.ShouldBe(Count);
    }
}
