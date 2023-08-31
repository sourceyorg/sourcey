using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sourcey.Extensions;
using Sourcey.Keys;

namespace Sourcey.Projections.InMemory;

internal class InMemoryDisposable : IAsyncDisposable
{
    public ValueTask DisposeAsync()
        => new();
}

internal sealed class InMemoryProjectionManager<TProjection> : IProjectionWriter<TProjection>, IProjectionReader<TProjection>
    where TProjection : class, IProjection, new()
{
    private readonly ConcurrentDictionary<string, TProjection> _projections = new();
    private readonly ILogger<InMemoryProjectionManager<TProjection>> _logger;

    public InMemoryProjectionManager(ILogger<InMemoryProjectionManager<TProjection>> logger)
    {
        _logger = logger;
    }

    public Task<TProjection> AddAsync(string subject, Func<TProjection> add, CancellationToken cancellationToken = default)
    {
        var entity = add();
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }

    public Task<TProjection> AddOrUpdateAsync(string subject, Action<TProjection> update, Func<TProjection> create, CancellationToken cancellationToken = default)
    {
        var entity = create();
        update(entity);
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }

    public ValueTask<IQueryableProjection<TProjection>> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return new(new QueryableProjection<TProjection>(_projections.Values.AsQueryable(), new InMemoryDisposable()));
    }

    public async ValueTask<IQueryableProjection<TProjection>> ReadAllWithConsistencyAsync(Subject subject, Func<TProjection?, bool> consistencyCheck, int retryCount = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(InMemoryProjectionManager<TProjection>)}.{nameof(ReadAllWithConsistencyAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        _ = await ReadWithConsistencyAsync(subject, consistencyCheck, retryCount, delay, cancellationToken);
        return await ReadAllAsync(cancellationToken);
    }

    public async ValueTask<IQueryableProjection<TProjection>> ReadAllWithConsistencyAsync(Func<IQueryable<TProjection>, ValueTask<bool>> consistencyCheckAsync, int retryCount = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(InMemoryProjectionManager<TProjection>)}.{nameof(ReadAllWithConsistencyAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var readFunc = (CancellationToken ct) => ReadAllAsync(ct);

        var (success, result) = await readFunc.WithRetryAsync(
            validityCheck: consistencyCheckAsync,
            retryCount: retryCount,
            delay: delay ?? TimeSpan.FromMilliseconds(50),
            cancellationToken: cancellationToken
        );

        if (!success)
        {
            _logger.LogWarning(
                "{service} failed to read consistent version of projections after {retryCount} retries",
                $"{nameof(InMemoryProjectionManager<TProjection>)}.{nameof(ReadAllWithConsistencyAsync)}",
                retryCount);
        }

        return result ?? await ReadAllAsync(cancellationToken);
    }

    public ValueTask<TProjection?> ReadAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(InMemoryProjectionManager<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        return new(_projections.TryGetValue(subject, out var projection) ? projection : null);
    }

    public async ValueTask<TProjection?> ReadWithConsistencyAsync(Subject subject, Func<TProjection?, bool> consistencyCheck, int retryCount = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(InMemoryProjectionManager<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var readFunc = (CancellationToken ct) => ReadAsync(subject, ct);

        var (success, result) = await readFunc.WithRetryAsync(
            validityCheck: consistencyCheck,
            retryCount: retryCount,
            delay: delay ?? TimeSpan.FromMilliseconds(50),
            cancellationToken: cancellationToken
        );

        if (!success)
        {
            _logger.LogWarning(
                "{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                $"{nameof(InMemoryProjectionManager<TProjection>)}.{nameof(ReadWithConsistencyAsync)}",
                subject,
                retryCount);
        }

        return result;
    }

    public Task RemoveAsync(string subject, CancellationToken cancellationToken = default)
    {
        _projections.TryRemove(subject, out _);
        return Task.CompletedTask;
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        _projections.Clear();
        return Task.CompletedTask;
    }

    public Task<TProjection> UpdateAsync(string subject, Func<TProjection, TProjection> update, CancellationToken cancellationToken = default)
    {
        var entity = update(_projections[subject]);
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }

    public Task<TProjection> UpdateAsync(string subject, Action<TProjection> update, CancellationToken cancellationToken = default)
    {
        var entity = _projections[subject];
        update(entity);
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }
}