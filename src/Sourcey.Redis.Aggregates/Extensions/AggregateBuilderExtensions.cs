using Microsoft.Extensions.DependencyInjection;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Builder;
using Sourcey.Redis.Aggregates.Snapshots;
using Sourcey.Extensions;

namespace Sourcey.Extensions
{
    public static class AggregateBuilderExtensions
    {
        public static IAggregateBuilder<TAggregate, TAggregateState> WithRedisSnapshotStrategy<TAggregate, TAggregateState>(
            this IAggregateBuilder<TAggregate, TAggregateState> builder,
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
