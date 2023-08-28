using Sourcey.Aggregates;
using Sourcey.Aggregates.Builder;
using Sourcey.Core.Builder;

namespace Sourcey.Extensions;

public static partial class SourceyBuilderExtensions
{
    public static ISourceyBuilder AddAggregate<TAggregate, TAggregateState>(this ISourceyBuilder builder, Action<IAggregateBuilder<TAggregate, TAggregateState>>? configuration = null)
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        var sourceyAggregateBuilder = new AggregateBuilder<TAggregate, TAggregateState>(builder.Services);
        
        if (configuration is not null)
            configuration(sourceyAggregateBuilder);

        return builder;
    }
}
