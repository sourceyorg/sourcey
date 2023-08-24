using Sourcey.Commands;
using Sourcey.Events;
using Sourcey.Events.Stores;
using Sourcey.Events.Streams;

namespace Sourcey.Aggregates.Stores
{
    public interface IAggregateStore<TEventStoreContext>
        where TEventStoreContext : IEventStoreContext
    {
        Task<TAggregate?> GetAsync<TAggregate, TState>(StreamId id, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
    }
}
