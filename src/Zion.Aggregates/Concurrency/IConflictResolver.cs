using Zion.Events;

namespace Zion.Aggregates.Concurrency
{
    public interface IConflictResolver
    {
        Task<ConflictAction> ResolveAsync<TAggregateState, TPrevEvent, TNextEvent, TConflictingEvent>(Aggregate<TAggregateState> aggregate, TPrevEvent? prevEvent, TNextEvent? nextEvent, TConflictingEvent? conflictingEvent)
            where TAggregateState : IAggregateState, new()
            where TPrevEvent : IEvent
            where TNextEvent : IEvent
            where TConflictingEvent: IEvent;
    }
}
