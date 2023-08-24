using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Sourcey.Aggregates.Snapshots;

namespace Sourcey.Redis.Aggregates.Tests.Snapshots.AggregateSnapshooter
{
    public class WhenSnapshotDoesntAlreadyExist : SnapshotSpecification<SnapshotAggregate, SnapshotAggregateState>
    {
        private readonly SnapshotAggregate _aggregate = new(new());
        public WhenSnapshotDoesntAlreadyExist(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override async Task Given()
        {
            var snapshooter = ServiceProvider.GetRequiredService<IAggregateSnapshooter<SnapshotAggregateState>>();
            await snapshooter.SaveAsync(_aggregate);
        }

        protected override Task When() => Task.CompletedTask;

        [Then]
        public async Task ThenSnapshotShouldBeSaved()
        {
            var connectionMultiplexer = ServiceProvider.GetRequiredService<IConnectionMultiplexerFactory>();
            var connection = connectionMultiplexer.Create(_redisContainer.GetConnectionString());
            var db = connection.GetDatabase();

            string? redisCache = await db.StringGetAsync(_aggregate.Id.ToString());

            redisCache.ShouldNotBeNull()
                .ShouldSatisfyAllConditions(
                    s => s.ShouldContain(@$"""id"":""{_aggregate.Id}"""),
                    s => s.ShouldContain(@$"""version"":{_aggregate.Version}"));
        }
    }
}
