using Sourcey.Events.Streams;

namespace Sourcey.Aggregates.Snapshots
{
    public interface IAggregateSnapshot<TAggregate, TState>
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
    {
        Task<TAggregate?> GetAsync(StreamId stream, Func<TAggregate, CancellationToken, Task<TAggregate>> rehydrateAsync, CancellationToken cancellationToken = default);
    }
}
