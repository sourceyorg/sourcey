using Microsoft.Extensions.DependencyInjection;
using Sourcey.Aggregates.Concurrency;
using Sourcey.Aggregates.Snapshots;
using Sourcey.Events;

namespace Sourcey.Aggregates.Builder;

public interface IAggregateBuilder<TAggregate, TAggregateState>
    where TAggregate : Aggregate<TAggregateState>
    where TAggregateState : IAggregateState, new()
{   
    IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution>()
        where TConflictResolution : class, IConflictResolution<TAggregateState>;
    IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TEvent>()
        where TConflictResolution : class, IConflictResolution<TAggregateState, TEvent>
        where TEvent : IEvent;
    IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TPrevEvent, TNextEvent>()
        where TConflictResolution : class, IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
        where TPrevEvent : IEvent
        where TNextEvent : IEvent;
    IAggregateBuilder<TAggregate, TAggregateState> WithSnapshotStrategy<TSnapshot, TSnapshooter>(SnapshotExecution execution)
        where TSnapshot : class, IAggregateSnapshot<TAggregate, TAggregateState>
        where TSnapshooter : class, IAggregateSnapshooter<TAggregateState>;
    IAggregateBuilder<TAggregate, TAggregateState> WithAutoResolution(Action<IAggregateAutoResolverBuilder<TAggregateState>> configuration);
}
