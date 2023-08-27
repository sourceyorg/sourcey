using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace Sourcey.Core.Stores;

public abstract class BufferedStore<TItem> : BackgroundService
{
    private readonly Channel<TItem> _itemQueue = Channel.CreateUnbounded<TItem>(new UnboundedChannelOptions
    {
        SingleWriter = false,
        SingleReader = true
    });

    public Task SaveAsync(TItem item, CancellationToken cancellationToken = default)
        => InternalSaveAsync(item, cancellationToken);

    protected async Task InternalSaveAsync(TItem item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _itemQueue.Writer.WriteAsync(item, cancellationToken);
    }

    protected abstract Task ConsumeAsync(TItem item, CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach(var item in _itemQueue.Reader.ReadAllAsync(stoppingToken))
            await ConsumeAsync(item, stoppingToken);
    }
}
