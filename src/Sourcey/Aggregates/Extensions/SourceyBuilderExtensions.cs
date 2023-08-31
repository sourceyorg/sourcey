using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Builder;
using Sourcey.Aggregates.Stores;
using Sourcey.Builder;

namespace Sourcey.Extensions;

/// <summary>
/// Extension methods for configuring and adding aggregates to the Sourcey builder.
/// </summary>
public static partial class SourceyBuilderExtensions
{
    /// <summary>
    /// Adds an aggregate to the Sourcey builder.
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TAggregateState">The type of the aggregate state.</typeparam>
    /// <param name="builder">The Sourcey builder.</param>
    /// <param name="configuration">An optional configuration action for the aggregate builder.</param>
    /// <returns>The Sourcey builder.</returns>
    /// </summary>
    public static ISourceyBuilder AddAggregate<TAggregate, TAggregateState>(this ISourceyBuilder builder, Action<IAggregateBuilder<TAggregate, TAggregateState>>? configuration = null)
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        builder.Services.TryAddScoped<IAggregateStore<TAggregate, TAggregateState>, AggregateStore<TAggregate, TAggregateState>>();
        var sourceyAggregateBuilder = new AggregateBuilder<TAggregate, TAggregateState>(builder.Services);
        
        
        if (configuration is not null)
            configuration(sourceyAggregateBuilder);

        return builder;
    }
}
