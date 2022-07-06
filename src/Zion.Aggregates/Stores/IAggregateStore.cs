using Zion.Commands;
using Zion.Events;
using Zion.Events.Stores;
using Zion.Events.Streams;

namespace Zion.Aggregates.Stores
{
    public interface IAggregateStore<TEventStoreContext>
        where TEventStoreContext : IEventStoreContext
    {
        Task<TAggregate> GetAsync<TAggregate, TState>(StreamId id, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
    }
}
