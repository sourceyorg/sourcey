using Sourcey.Events;

namespace Sourcey.Aggregates.Concurrency;

public interface IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
        where TAggregateState : IAggregateState, new()
        where TPrevEvent : IEvent
        where TNextEvent : IEvent
{
    Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate, TPrevEvent? prevEvent, TNextEvent? nextEvent);
}

public interface IConflictResolution<TAggregateState, TEvent>
        where TAggregateState : IAggregateState, new()
        where TEvent : IEvent
{
    Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate, TEvent @event);
}

public interface IConflictResolution<TAggregateState>
        where TAggregateState : IAggregateState, new()
{
    Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate);
}
