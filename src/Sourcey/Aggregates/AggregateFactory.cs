using Sourcey.Events;

namespace Sourcey.Aggregates;

/// <summary>
/// <inheritdoc/>
/// </summary>
internal sealed class AggregateFactory : IAggregateFactory
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent>? events = null)
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
    {
        events ??= Enumerable.Empty<IEvent>();
        
        var aggregate = (TAggregate?)Activator.CreateInstance(typeof(TAggregate), new object[] { new TState() });

        if (aggregate is null)
            throw new InvalidOperationException();
        
        aggregate.FromHistory(events);

        return aggregate;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TAggregate Create<TAggregate, TState>()
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
        => FromHistory<TAggregate, TState>(null);
}
