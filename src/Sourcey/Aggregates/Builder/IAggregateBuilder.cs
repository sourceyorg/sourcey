using Sourcey.Aggregates.Concurrency;
using Sourcey.Aggregates.Snapshots;
using Sourcey.Events;

namespace Sourcey.Aggregates.Builder;

/// <summary>
/// A builder to configure an aggregate.
/// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
/// <typeparam name="TAggregateState">The type of the aggregate state.</typeparam>
/// </summary>
public interface IAggregateBuilder<TAggregate, TAggregateState>
    where TAggregate : Aggregate<TAggregateState>
    where TAggregateState : IAggregateState, new()
{   
    /// <summary>
    /// Configures the aggregate to use the specified conflict resolution.
    /// <typeparam name="TConflictResolution">The type of conflict resolution</typeparam>
    /// <returns>The builder</returns>
    /// </summary>
    IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution>()
        where TConflictResolution : class, IConflictResolution<TAggregateState>;

    /// <summary>
    /// Configures the aggregate to use the specified conflict resolution.
    /// <typeparam name="TConflictResolution">The type of conflict resolution</typeparam>
    /// <typeparamref name="TEvent">The type of event to resolve</typeparamref>
    /// <returns>The builder</returns>
    /// </summary>
    IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TEvent>()
        where TConflictResolution : class, IConflictResolution<TAggregateState, TEvent>
        where TEvent : IEvent;

    /// <summary>
    /// Configures the aggregate to use the specified conflict resolution.
    /// <typeparam name="TConflictResolution">The type of conflict resolution</typeparam>
    /// <typeparamref name="TPrevEvent">The type of previous event to resolve</typeparamref>
    /// <typeparamref name="TNextEvent">The type of next event to resolve</typeparamref>
    /// <returns>The builder</returns>
    /// </summary>
    IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TPrevEvent, TNextEvent>()
        where TConflictResolution : class, IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
        where TPrevEvent : IEvent
        where TNextEvent : IEvent;

    /// <summary>
    /// Configures the aggregate to use the specified snapshot strategy
    /// <typeparamref name="TSnapshot">The type ofo snapshot</typeparamref>
    /// <typeparamref name="TSnapshooter">The type of snapshooter</typeparamref>
    /// </summary>
    IAggregateBuilder<TAggregate, TAggregateState> WithSnapshotStrategy<TSnapshot, TSnapshooter>(SnapshotExecution execution)
        where TSnapshot : class, IAggregateSnapshot<TAggregate, TAggregateState>
        where TSnapshooter : class, IAggregateSnapshooter<TAggregateState>;
    
    /// <summary>
    /// Configures the aggrgate to use auto conflict resolution.
    /// <param name="configuration">Configuration of the auto conflict resolution</param>
    /// </summary>
    IAggregateBuilder<TAggregate, TAggregateState> WithAutoResolution(Action<IAggregateAutoResolverBuilder<TAggregateState>> configuration);
}
