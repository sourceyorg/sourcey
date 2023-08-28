using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Sourcey.Keys;

namespace Sourcey.Aggregates.Snapshots;

internal sealed class BufferedAggregateSnapshooter<TState> : BackgroundService, IAggregateSnapshooter<TState>
    where TState : IAggregateState, new()
{
    private readonly ConcurrentDictionary<StreamId, ConcurrentQueue<Aggregate<TState>>> _queues = new();

    private readonly IAggregateSnapshooter<TState> _aggregateSnapshooter;

    public BufferedAggregateSnapshooter(IAggregateSnapshooter<TState> aggregateSnapshooter)
    {
        if (aggregateSnapshooter is null)
            throw new ArgumentNullException(nameof(aggregateSnapshooter));

        _aggregateSnapshooter = aggregateSnapshooter;
    }

    public Task SaveAsync(Aggregate<TState> aggregate, CancellationToken cancellationToken = default)
    {
        QueueSnapshot(aggregate, cancellationToken);
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var snapshots = DequeueSnapshots(stoppingToken);
        await Task.WhenAll(snapshots.Select(s => _aggregateSnapshooter.SaveAsync(s, stoppingToken)));
    }

    private IEnumerable<Aggregate<TState>> DequeueSnapshots(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        foreach (var queue in _queues.Values.ToArray())
            if (queue.TryDequeue(out var aggregate))
                yield return aggregate;
    }

    private void QueueSnapshot(Aggregate<TState> aggregate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _queues.AddOrUpdate(
            key: aggregate.Id,
            addValueFactory: _ => new(new Aggregate<TState>[] { aggregate }),
            updateValueFactory: (_, queue) => {
                queue.Enqueue(aggregate);
                return queue;
            }
        );
    }
}
