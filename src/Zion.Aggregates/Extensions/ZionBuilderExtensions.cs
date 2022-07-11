using Zion.Aggregates.Builder;
using Zion.Core.Builder;

namespace Zion.Aggregates.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddAggregate<TAggregate, TAggregateState>(this IZionBuilder builder, Action<IZionAggregateBuilder<TAggregate, TAggregateState>> configuration)
            where TAggregate : Aggregate<TAggregateState>
            where TAggregateState : IAggregateState, new()
        {
            var zionAggregateBuilder = new ZionAggregateBuilder<TAggregate, TAggregateState>(builder.Services);
            configuration(zionAggregateBuilder);
            return builder;
        }
    }
}
