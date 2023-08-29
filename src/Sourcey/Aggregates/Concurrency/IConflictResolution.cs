using Sourcey.Events;

namespace Sourcey.Aggregates.Concurrency;

/// <summary>
/// Interface for resolving conflicts in aggregate events.
/// <typeparam name="TAggregateState">The type of the aggregate state.</typeparam>
/// <typeparam name="TPrevEvent">The type of the previous event.</typeparam>
/// <typeparam name="TNextEvent">The type of the next event.</typeparam>
/// </summary>
public interface IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
        where TAggregateState : IAggregateState, new()
        where TPrevEvent : IEvent
        where TNextEvent : IEvent
{
    /// <summary>
    /// Resolves conflicts between previous and next events in an aggregate.
    /// <param name="aggregate">The aggregate to resolve conflicts for.</param>
    /// <param name="prevEvent">The previous event.</param>
    /// <param name="nextEvent">The next event.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that returns a <see cref="ConflictAction"/>.</returns>
    /// </summary>
    Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate, TPrevEvent? prevEvent, TNextEvent? nextEvent);
}

/// <summary>
/// Interface for resolving conflicts in aggregate events.
/// <typeparam name="TAggregateState">The type of the aggregate state.</typeparam>
/// <typeparam name="TEvent">The type of the event.</typeparam>
/// </summary>
public interface IConflictResolution<TAggregateState, TEvent>
        where TAggregateState : IAggregateState, new()
        where TEvent : IEvent
{
    /// <summary>
    /// Resolves conflicts in an aggregate event.
    /// <param name="aggregate">The aggregate to resolve conflicts for.</param>
    /// <param name="event">The event to resolve conflicts for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that returns a <see cref="ConflictAction"/>.</returns>
    /// </summary>
    Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate, TEvent @event);
}

/// <summary>
/// Interface for resolving conflicts in aggregate events.
/// <typeparam name="TAggregateState">The type of the aggregate state.</typeparam>
/// </summary>
public interface IConflictResolution<TAggregateState>
        where TAggregateState : IAggregateState, new()
{
    /// <summary>
    /// Resolves conflicts in an aggregate.
    /// <param name="aggregate">The aggregate to resolve conflicts for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation that returns a <see cref="ConflictAction"/>.</returns>
    /// </summary>
    Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate);
}
