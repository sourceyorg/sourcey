using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Sourcey.Extensions;
using Sourcey.Keys;

namespace Sourcey.Projections.InMemory;

internal class InMemoryDisposable : IAsyncDisposable
{
    public ValueTask DisposeAsync()
        => new();
}

internal sealed class InMemoryProjectionManager<TProjection> : IProjectionWriter<TProjection>,
    IProjectionReader<TProjection>
    where TProjection : class, IProjection, new()
{
    private readonly ConcurrentDictionary<string, TProjection> _projections = new();
    private readonly ILogger<InMemoryProjectionManager<TProjection>> _logger;

    public InMemoryProjectionManager(ILogger<InMemoryProjectionManager<TProjection>> logger)
    {
        _logger = logger;
    }

    public Task<TProjection> AddAsync(string subject, Func<TProjection> add,
        CancellationToken cancellationToken = default)
    {
        var entity = add();
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }

    public Task<TProjection> AddOrUpdateAsync(string subject, Action<TProjection> update, Func<TProjection> create,
        CancellationToken cancellationToken = default)
    {
        var entity = create();
        update(entity);
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }

    public ValueTask<IQueryableProjection<TProjection>> QueryAsync(CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        return new(new QueryableProjection<TProjection>(_projections.Values.AsQueryable(), new InMemoryDisposable()));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }

    public async ValueTask<IQueryableProjection<TProjection>> QueryAsync(Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck, int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, consistencyCheck, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                nameof(InMemoryProjectionManager<TProjection>),
                nameof(QueryAsync),
                subject,
                retryCount);
        }

        return await QueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<IQueryableProjection<TProjection>> QueryAsync(Subject subject, int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, null, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                nameof(InMemoryProjectionManager<TProjection>),
                nameof(QueryAsync),
                subject,
                retryCount);
        }

        return await QueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<IQueryableProjection<TProjection>> QueryAsync(
        Func<IQueryable<TProjection?>, ValueTask<bool>> consistencyCheckAsync, int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        Func<Task<bool>> readFunc = async () =>
        {
            var query = await QueryAsync(cancellationToken).ConfigureAwait(false);
            return await consistencyCheckAsync(query).ConfigureAwait(false);
        };

        var success = await readFunc.WithRetryAsync(
            retryCount: retryCount,
            delay: delay ?? TimeSpan.FromMilliseconds(50),
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection after {retryCount} retries",
                nameof(InMemoryProjectionManager<TProjection>),
                nameof(QueryAsync),
                retryCount);
        }

        return await QueryAsync(cancellationToken).ConfigureAwait(false);
    }


    public ValueTask<TProjection?> ReadAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(InMemoryProjectionManager<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        return new(_projections.TryGetValue(subject, out var projection) ? projection : null);
    }

    public async ValueTask<TResult?> ReadAsync<TResult>(Subject subject,
        Expression<Func<TProjection, TResult>> projection,
        CancellationToken cancellationToken = default)
    {
        var result = await ReadAsync(subject, cancellationToken).ConfigureAwait(false);

        if (result is null)
            return default;

        return projection.Compile()(result);
    }

    public async ValueTask<TResult?> ReadAsync<TResult>(Subject subject,
        Expression<Func<TProjection, TResult>> projection,
        int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, null, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                nameof(InMemoryProjectionManager<TProjection>),
                nameof(QueryAsync),
                subject,
                retryCount);
        }

        return await ReadAsync(subject, projection, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<TProjection?> ReadAsync(Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck,
        int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, consistencyCheck, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                nameof(InMemoryProjectionManager<TProjection>),
                nameof(QueryAsync),
                subject,
                retryCount);
        }

        return await ReadAsync(subject, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<TProjection?> ReadAsync(Subject subject, int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, null, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                nameof(InMemoryProjectionManager<TProjection>),
                nameof(QueryAsync),
                subject,
                retryCount);
        }

        return await ReadAsync(subject, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<TResult?> ReadAsync<TResult>(Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck, Expression<Func<TProjection, TResult>> projection,
        int retryCount = 3,
        TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, consistencyCheck, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                nameof(InMemoryProjectionManager<TProjection>),
                nameof(QueryAsync),
                subject,
                retryCount);
        }

        return await ReadAsync(subject, projection, cancellationToken).ConfigureAwait(false);
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

    public Task<TProjection> UpdateAsync(string subject, Func<TProjection, TProjection> update,
        CancellationToken cancellationToken = default)
    {
        var entity = update(_projections[subject]);
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }

    public Task<TProjection> UpdateAsync(string subject, Action<TProjection> update,
        CancellationToken cancellationToken = default)
    {
        var entity = _projections[subject];
        update(entity);
        _projections.AddOrUpdate(subject, _ => entity, (_, _) => entity);
        return Task.FromResult(entity);
    }

    private async Task<bool> ConsistencyCheckAsync(Subject subject,
        Expression<Func<TProjection?, bool>>? consistencyCheck, int retryCount,
        TimeSpan? delay, CancellationToken cancellationToken)
    {
        var readFunc = BuildConsistencyCheck(subject, consistencyCheck, cancellationToken);

        return await readFunc.WithRetryAsync(
            retryCount: retryCount,
            delay: delay ?? TimeSpan.FromMilliseconds(50),
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);
    }

    private Func<Task<bool>> BuildConsistencyCheck(Subject subject,
        Expression<Func<TProjection?, bool>>? consistencyCheck, CancellationToken cancellationToken)
    {
        return ReadFunc;

        Task<bool> ReadFunc()
        {
            var query = _projections
                .Where(s => s.Key == subject.ToString());

            if (consistencyCheck is not null)
            {
                var compiledCheck = consistencyCheck.Compile();
                query = query.Where(s => compiledCheck(s.Value));
            }


            return Task.FromResult(query.Any());
        }
    }
}
