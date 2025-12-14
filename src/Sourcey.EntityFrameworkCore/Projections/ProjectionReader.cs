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

        var context = _projectionDbContextFactory.Create<TProjection>();
        await using (context.ConfigureAwait(false))
        {
            return await context.Set<TProjection>()
                .AsQueryable()
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefaultAsync(s => s.Subject == subject.ToString(), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public async ValueTask<TResult?> ReadAsync<TResult>(Subject subject,
        Expression<Func<TProjection, TResult>> projection, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var context = _projectionDbContextFactory.Create<TProjection>();
        await using (context.ConfigureAwait(false))
        {
            return await context.Set<TProjection>()
                .AsQueryable()
                .AsNoTrackingWithIdentityResolution()
                .Where(s => s.Subject == subject.ToString())
                .Select(projection)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask<TProjection?> ReadAsync(Subject subject, int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
        => ReadAsync(subject: subject, consistencyCheck: null, retryCount: retryCount, delay: delay,
            cancellationToken: cancellationToken);

    public async ValueTask<TProjection?> ReadAsync(Subject subject,
        Expression<Func<TProjection?, bool>>? consistencyCheck, int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, consistencyCheck, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (success)
            return await ReadAsync(subject, cancellationToken).ConfigureAwait(false);

        _logger.LogWarning(
            "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
            nameof(ProjectionReader<TProjection>),
            nameof(ReadAsync),
            subject,
            retryCount);

        return default;
    }

    public ValueTask<TResult?> ReadAsync<TResult>(Subject subject, Expression<Func<TProjection, TResult>> projection,
        int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
        => ReadAsync(subject: subject, projection: projection, consistencyCheck: null, retryCount: retryCount,
            delay: delay, cancellationToken: cancellationToken);

    public async ValueTask<TResult?> ReadAsync<TResult>(Subject subject,
        Expression<Func<TProjection?, bool>>? consistencyCheck, Expression<Func<TProjection, TResult>> projection,
        int retryCount = 3,
        TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, consistencyCheck, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (success)
            return await ReadAsync(subject, projection, cancellationToken).ConfigureAwait(false);

        _logger.LogWarning(
            "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
            nameof(ProjectionReader<TProjection>),
            nameof(ReadAsync),
            subject,
            retryCount);

        return default;
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

#pragma warning disable CA2000 // Dispose objects before losing scope
        return new ValueTask<IQueryableProjection<TProjection>>(new QueryableProjection<TProjection>(
            context.Set<TProjection>().AsQueryable().AsNoTracking(),
            context)
        );
#pragma warning restore CA2000 // Dispose objects before losing scope
    }

    public ValueTask<IQueryableProjection<TProjection>> QueryAsync(Subject subject, int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
        => QueryAsync(subject: subject, consistencyCheck: null, retryCount: retryCount, delay: delay,
            cancellationToken: cancellationToken);

    public async ValueTask<IQueryableProjection<TProjection>> QueryAsync(Subject subject,
        Expression<Func<TProjection?, bool>>? consistencyCheck, int retryCount = 3, TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        var success = await ConsistencyCheckAsync(subject, consistencyCheck, retryCount, delay, cancellationToken)
            .ConfigureAwait(false);

        if (!success)
        {
            _logger.LogWarning(
                "{reader}.{service} failed to read consistent version of projection with subject {subject} after {retryCount} retries",
                nameof(ProjectionReader<TProjection>),
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
            var context = _projectionDbContextFactory.Create<TProjection>();
            await using (context.ConfigureAwait(false))
            {
                IQueryable<TProjection?> query = context.Set<TProjection>()
                    .AsQueryable()
                    .AsNoTrackingWithIdentityResolution();


                return await consistencyCheckAsync(query).ConfigureAwait(false);
            }
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
                nameof(ProjectionReader<TProjection>),
                nameof(QueryAsync),
                retryCount);
        }

        return await QueryAsync(cancellationToken).ConfigureAwait(false);
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

        async Task<bool> ReadFunc()
        {
            var context = _projectionDbContextFactory.Create<TProjection>();
            await using (context.ConfigureAwait(false))
            {
                IQueryable<TProjection?> query = context.Set<TProjection>()
                    .AsQueryable()
                    .AsNoTrackingWithIdentityResolution()
                    .Where(s => s.Subject == subject.ToString());

                if (consistencyCheck is not null) query = query.Where(consistencyCheck);


                return await query.AnyAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
