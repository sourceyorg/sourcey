namespace Sourcey.Aggregates.Snapshots;

public interface IAggregateSnapshooter<TState>
    where TState : IAggregateState, new()
{
    Task SaveAsync(Aggregate<TState> aggregate, CancellationToken cancellationToken = default);
}
