using Sourcey.Keys;
using Sourcey.Events;
using Sourcey.Events.Stores;

namespace Sourcey.Aggregates.Stores;

public interface IAggregateStore<TAggregate, TState>
    where TState : IAggregateState, new()
    where TAggregate : Aggregate<TState>
{
    Task<TAggregate?> GetAsync(
        StreamId id,
        CancellationToken cancellationToken = default);
    Task SaveAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);
    Task SaveAsync(
        TAggregate aggregate,
        Actor? actor = null,
        Causation? causation = null,
        Correlation? correlation = null,
        DateTimeOffset? scheduledPublication = null,
        int? expectedVersion = null,
        CancellationToken cancellationToken = default);
    Task SaveAsync(
        TAggregate aggregate,
        IEventContext<IEvent> causation,
        int? expectedVersion = null,
        CancellationToken cancellationToken = default);
    Task SaveAsync(
        TAggregate aggregate,
        IEventContext<IEvent> causation,
        CancellationToken cancellationToken = default);
}
