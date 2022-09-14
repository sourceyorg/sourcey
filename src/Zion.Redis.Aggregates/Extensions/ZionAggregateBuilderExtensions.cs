using Microsoft.Extensions.DependencyInjection;
using Zion.Aggregates;
using Zion.Aggregates.Builder;
using Zion.Redis.Aggregates.Snapshots;
using Zion.Extensions;

namespace Zion.Extensions
{
    public static class ZionAggregateBuilderExtensions
    {
        public static IZionAggregateBuilder<TAggregate, TAggregateState> WithRedisSnapshotStrategy<TAggregate, TAggregateState>(
            this IZionAggregateBuilder<TAggregate, TAggregateState> builder,
            Action<AggregateSnapshotSettings<TAggregateState>> options)
            where TAggregate : Aggregate<TAggregateState>
            where TAggregateState : IAggregateState, new()
        {
            builder.Services.Configure(options);

            var tempOptions = new AggregateSnapshotSettings<TAggregateState>();
            options(tempOptions);

            builder.Services.AddRedisConnectionFactory();
            builder.WithSnapshotStrategy<AggregateSnapshot<TAggregate, TAggregateState>, AggregateSnapshooter<TAggregateState>>(tempOptions.SnapshotExecution);

            return builder;
        } 
    }
}
