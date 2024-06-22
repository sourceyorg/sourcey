using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Readonly;
using Sourcey.Keys;
using Sourcey.Projections;
using Sourcey.Extensions;

namespace Sourcey.EntityFrameworkCore.Projections;

internal sealed class ProjectionReader<TProjection> : IProjectionReader<TProjection>
    where TProjection : class, IProjection
{
    private readonly ILogger<ProjectionReader<TProjection>> _logger;
    private readonly IReadonlyProjectionDbContextFactory _projectionDbContextFactory;


    public ProjectionReader(ILogger<ProjectionReader<TProjection>> logger,
        IReadonlyProjectionDbContextFactory projectionDbContextFactory)
    {
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));
        if (projectionDbContextFactory == null)
            throw new ArgumentNullException(nameof(projectionDbContextFactory));

        _logger = logger;
        _projectionDbContextFactory = projectionDbContextFactory;
    }

    public async ValueTask<TProjection?> ReadAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        await using var context = _projectionDbContextFactory.Create<TProjection>();

        return await context.Set<TProjection>()
            .AsQueryable()
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(s => s.Subject == subject.ToString(), cancellationToken: cancellationToken);
    }

    public async ValueTask<TResult?> ReadAsync<TResult>(Subject subject,
        Expression<Func<TProjection, TResult>> projection,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        await using var context = _projectionDbContextFactory.Create<TProjection>();

        return await context.Set<TProjection>()
            .AsQueryable()
            .AsNoTrackingWithIdentityResolution()
            .Where(s => s.Subject == subject.ToString())
            .Select(projection)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask<TProjection?> ReadWithConsistencyAsync(
        Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var readFunc = async (CancellationToken ct) =>
        {
            await using var context = _projectionDbContextFactory.Create<TProjection>();

            var result = await context.Set<TProjection>()
                .AsQueryable()
                .AsNoTrackingWithIdentityResolution()
                .Where(s => s.Subject == subject.ToString())
                .Where(consistencyCheck)
                .AnyAsync(cancellationToken: ct);

            return result;
        };


        var (success, _) = await readFunc.WithRetryAsync(
            validityCheck: t => t,
            retryCount: retryCount,
            delay: delay ?? TimeSpan.FromMilliseconds(50),
            cancellationToken: cancellationToken
        );
        
        if (!success)
        {
            _logger.LogWarning(
                "{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadWithConsistencyAsync)}",
                subject,
                retryCount);

            return default;
        }

        return await ReadAsync(subject, cancellationToken);
    }

    public async ValueTask<TResult?> ReadWithConsistencyAsync<TResult>(Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck,
        Expression<Func<TProjection, TResult>> projection,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadWithConsistencyAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var readFunc = async (CancellationToken ct) =>
        {
            await using var context = _projectionDbContextFactory.Create<TProjection>();

            var result = await context.Set<TProjection>()
                .AsQueryable()
                .AsNoTrackingWithIdentityResolution()
                .Where(s => s.Subject == subject.ToString())
                .Where(consistencyCheck)
                .AnyAsync(cancellationToken: ct);

            return result;
        };


        var (success, _) = await readFunc.WithRetryAsync(
            validityCheck: t => t,
            retryCount: retryCount,
            delay: delay ?? TimeSpan.FromMilliseconds(50),
            cancellationToken: cancellationToken
        );
        
        if (!success)
        {
            _logger.LogWarning(
                "{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadWithConsistencyAsync)}",
                subject,
                retryCount);

            return default;
        }

        return await ReadAsync(subject, projection, cancellationToken);
    }

    public ValueTask<IQueryableProjection<TProjection>> QueryAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(QueryAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var context = _projectionDbContextFactory.Create<TProjection>();

        return new ValueTask<IQueryableProjection<TProjection>>(new QueryableProjection<TProjection>(
            context.Set<TProjection>().AsQueryable().AsNoTracking(),
            context)
        );
    }


    public async ValueTask<IQueryableProjection<TProjection>> QueryWithConsistencyAsync(
        Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(QueryWithConsistencyAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        _ = await ReadWithConsistencyAsync(subject, consistencyCheck, retryCount, delay, cancellationToken);
        return await QueryAsync(cancellationToken);
    }

    public async ValueTask<IQueryableProjection<TProjection>> QueryWithConsistencyAsync(
        Func<IQueryable<TProjection>, ValueTask<bool>> consistencyCheckAsync,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(QueryWithConsistencyAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var readFunc = (CancellationToken ct) => QueryAsync(ct);

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
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(QueryWithConsistencyAsync)}",
                retryCount);
        }

        return result ?? await QueryAsync(cancellationToken);
    }
}
