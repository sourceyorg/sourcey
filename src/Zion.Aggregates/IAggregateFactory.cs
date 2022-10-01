using Zion.Events;

namespace Zion.Aggregates
{
    public interface IAggregateFactory
    {
        TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent>? events = null)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();

        TAggregate Create<TAggregate, TState>()
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
    }
}
