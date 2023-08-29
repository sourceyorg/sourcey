using Sourcey.Events;

namespace Sourcey.Aggregates.Concurrency;



/// <summary>
/// Represents the action to take to resolve a conflict.
/// </summary>
public interface IConflictResolver
{
    /// <summary>
    /// Resolves conflicts between events in an aggregate.
    /// <typeparam name="TAggregateState">The type of the aggregate state.</typeparam>
    /// <typeparam name="TPrevEvent">The type of the previous event.</typeparam>
    /// <typeparam name="TNextEvent">The type of the next event.</typeparam>
    /// <typeparam name="TConflictingEvent">The type of the conflicting event.</typeparam>
    /// <param name="aggregate">The aggregate.</param>
    /// <param name="prevEvent">The previous event.</param>
    /// <param name="nextEvent">The next event.</param>
    /// <param name="conflictingEvent">The conflicting event.</param>
    /// <returns>The action to take to resolve the conflict.</returns>
    /// </summary>
    Task<ConflictAction> ResolveAsync<TAggregateState, TPrevEvent, TNextEvent, TConflictingEvent>(Aggregate<TAggregateState> aggregate, TPrevEvent? prevEvent, TNextEvent? nextEvent, TConflictingEvent? conflictingEvent)
        where TAggregateState : IAggregateState, new()
        where TPrevEvent : IEvent
        where TNextEvent : IEvent
        where TConflictingEvent: IEvent;
}
