using Zion.Events;

namespace Zion.Aggregates
{
    internal sealed class AggregateFactory : IAggregateFactory
    {
        public TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent> events)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            
            var aggregate = (TAggregate?)Activator.CreateInstance(typeof(TAggregate), new object[] { new TState() });

            if (aggregate is null)
                throw new InvalidOperationException();
            
            aggregate.FromHistory(events);

            return aggregate;
        }
    }
}
