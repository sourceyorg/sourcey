using Sourcey.Events;

namespace Sourcey.Aggregates.Concurrency;

internal sealed class AutoConflictResolution<TAggregateState, TPrevEvent, TNextEvent> : IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
        where TAggregateState : IAggregateState, new()
        where TPrevEvent : IEvent
        where TNextEvent : IEvent
{
    public Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate, TPrevEvent? prevEvent, TNextEvent? nextEvent)
        => Task.FromResult(ConflictAction.Pass);
}

internal sealed class AutoConflictResolution<TAggregateState, TEvent> : IConflictResolution<TAggregateState, TEvent>
        where TAggregateState : IAggregateState, new()
        where TEvent : IEvent
{
    public Task<ConflictAction> ResolveAsync(Aggregate<TAggregateState> aggregate, TEvent @event)
        => Task.FromResult(ConflictAction.Pass);
}
