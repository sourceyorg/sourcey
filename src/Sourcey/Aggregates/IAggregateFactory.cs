using Sourcey.Events;

namespace Sourcey.Aggregates;

/// <summary>
/// Factory for creating aggregates.
/// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
/// <typeparam name="TState">The type of the aggregate state.</typeparam>
/// </summary>
public interface IAggregateFactory
{
    /// <summary>
    /// Creates a new aggregate from the specified events.
    /// <typeparam name="TAggregate"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <param name="events">Events to be played into the aggreagte</param>
    /// <returns>An aggreagte with the events applied</returns>
    TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent>? events = null)
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new();

    /// <summary>
    /// Creates a new aggregate.
    /// <typeparam name="TAggregate"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <returns>A new aggreagte</returns>
    /// </summary>
    TAggregate Create<TAggregate, TState>()
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new();
}
