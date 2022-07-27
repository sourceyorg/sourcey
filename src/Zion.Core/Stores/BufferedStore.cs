using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;

namespace Zion.Core.Stores
{
    public abstract class BufferedStore<TItem> : BackgroundService
    {
        private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(5));
        private readonly ConcurrentQueue<TItem> _itemQueue = new();

        public Task SaveAsync(TItem item, CancellationToken cancellationToken = default)
            => InternalSaveAsync(item, cancellationToken);

        protected Task InternalSaveAsync(TItem item, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _itemQueue.Enqueue(item);
            return Task.CompletedTask;
        }

        protected abstract Task ConsumeAsync(TItem item, CancellationToken cancellationToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _periodicTimer.WaitForNextTickAsync(stoppingToken))
                while(_itemQueue.Any())
                    if (_itemQueue.TryDequeue(out var item))
                        await ConsumeAsync(item, stoppingToken);
        }
    }
}
