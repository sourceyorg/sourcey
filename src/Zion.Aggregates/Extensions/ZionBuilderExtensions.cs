using Zion.Aggregates.Builder;
using Zion.Core.Builder;

namespace Zion.Aggregates.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionAggregateBuilder<TAggregate, TAggregateState> AddAggregate<TAggregate, TAggregateState>(this IZionBuilder builder)
            where TAggregate : Aggregate<TAggregateState>
            where TAggregateState : IAggregateState, new()
            => new ZionAggregateBuilder<TAggregate, TAggregateState>(builder.Services);
    }
}
