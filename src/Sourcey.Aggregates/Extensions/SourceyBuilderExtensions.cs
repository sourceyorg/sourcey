using Sourcey.Aggregates;
using Sourcey.Aggregates.Builder;
using Sourcey.Core.Builder;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddAggregate<TAggregate, TAggregateState>(this ISourceyBuilder builder, Action<IAggregateBuilder<TAggregate, TAggregateState>> configuration)
            where TAggregate : Aggregate<TAggregateState>
            where TAggregateState : IAggregateState, new()
        {
            var sourceyAggregateBuilder = new AggregateBuilder<TAggregate, TAggregateState>(builder.Services);
            configuration(sourceyAggregateBuilder);
            return builder;
        }
    }
}
