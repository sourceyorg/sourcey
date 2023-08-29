using Sourcey.Keys;
using Sourcey.Events;
using Sourcey.Events.Stores;

namespace Sourcey.Aggregates.Stores;

/// <summary>
/// Store to retrieve and save aggregates.
/// <typeparam name="TEventStoreContext">The type of the event store context.</typeparam>
/// </summary>
public interface IAggregateStore<TEventStoreContext>
    where TEventStoreContext : IEventStoreContext
{
    /// <summary>
    /// Gets the aggregate with the specified id.
    /// <typeparam name="TAggregate"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <param name="id">Unique identifier of the aggreagte</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The aggreagte</returns>
    Task<TAggregate?> GetAsync<TAggregate, TState>(
        StreamId id,
        CancellationToken cancellationToken = default)
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new();

    /// <summary>
    /// Save the aggregate and store all uncommited events.
    /// <typeparam name="TState"></typeparam>
    /// <param name="aggregate">The aggregate to be saved</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    /// </summary>
    Task SaveAsync<TState>(
        Aggregate<TState> aggregate,
        CancellationToken cancellationToken = default)
        where TState : IAggregateState, new();

    /// <summary>
    /// Save the aggregate and store all uncommited events.
    /// <typeparam name="TState"></typeparam>
    /// <param name="aggregate">The aggregate to be saved</param>
    /// <param name="actor">The actor that caused the latest changes</param>
    /// <param name="causation">The causation of the latest changes</param>
    /// <param name="correlation">The correlation of the latest changes</param>
    /// <param name="scheduledPublication">The scheduled publication of the latest changes</param>
    /// <param name="expectedVersion">The expected version of the aggregate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    /// </summary>
    Task SaveAsync<TState>(
        Aggregate<TState> aggregate,
        Actor? actor = null,
        Causation? causation = null,
        Correlation? correlation = null,
        DateTimeOffset? scheduledPublication = null,
        int? expectedVersion = null,
        CancellationToken cancellationToken = default)
        where TState : IAggregateState, new();

    /// <summary>
    /// Save the aggregate and store all uncommited events.
    /// <typeparam name="TState"></typeparam>
    /// <param name="aggregate">The aggregate to be saved</param>
    /// <param name="causation">The causation of the latest changes</param>
    /// <param name="expectedVersion">The expected version of the aggregate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    /// </summary>
    Task SaveAsync<TState>(
        Aggregate<TState> aggregate,
        IEventContext<IEvent> causation,
        int? expectedVersion = null,
        CancellationToken cancellationToken = default)
        where TState : IAggregateState, new();

    /// <summary>
    /// Save the aggregate and store all uncommited events.
    /// <typeparam name="TState"></typeparam>
    /// <param name="aggregate">The aggregate to be saved</param>
    /// <param name="causation">The causation of the latest changes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    /// </summary>
    Task SaveAsync<TState>(
        Aggregate<TState> aggregate,
        IEventContext<IEvent> causation,
        CancellationToken cancellationToken = default)
        where TState : IAggregateState, new();
}
