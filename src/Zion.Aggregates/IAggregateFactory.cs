using Zion.Events;

namespace Zion.Aggregates
{
    public interface IAggregateFactory
    {
        TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent> events)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
    }
}
