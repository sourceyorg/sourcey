using Microsoft.Extensions.Logging;
using Zion.Core.Keys;
using Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Zion.Projections;
using Zion.Core.Extensions;

namespace Zion.EntityFrameworkCore.Projections
{
    internal sealed class ProjectionReader<TProjection> : IProjectionReader<TProjection>
        where TProjection : class, IProjection
    {
        private readonly ILogger<ProjectionReader<TProjection>> _logger;
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;


        public ProjectionReader(ILogger<ProjectionReader<TProjection>> logger,
            IProjectionDbContextFactory projectionDbContextFactory)
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
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _projectionDbContextFactory.Create<TProjection>();
            return await context.Set<TProjection>()
                .FindAsync(new object?[] { subject.ToString() }, cancellationToken: cancellationToken);
        }

        public async ValueTask<TProjection?> ReadWithConsistencyAsync(
            Subject subject,
            Func<TProjection?, bool> consistencyCheck,
            int retryCount = 3,
            TimeSpan? delay = null,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAsync)} was cancelled before execution");
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
                    $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadWithConsistencyAsync)}",
                    subject,
                    retryCount);
            }

            return result;
        }

        public ValueTask<IQueryableProjection<TProjection>> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAllAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var context = _projectionDbContextFactory.Create<TProjection>();
            
            return new ValueTask<IQueryableProjection<TProjection>>(new QueryableProjection<TProjection>(
                context.Set<TProjection>(),
                context)
            );
        }


        public async ValueTask<IQueryableProjection<TProjection>> ReadAllWithConsistencyAsync(
            Subject subject,
            Func<TProjection?, bool> consistencyCheck,
            int retryCount = 3,
            TimeSpan? delay = null,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAllWithConsistencyAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            _ = await ReadWithConsistencyAsync(subject, consistencyCheck, retryCount, delay, cancellationToken);
            return await ReadAllAsync(cancellationToken);
        }

        public async ValueTask<IQueryableProjection<TProjection>> ReadAllWithConsistencyAsync(
            Func<IQueryable<TProjection>, ValueTask<bool>> consistencyCheckAsync,
            int retryCount = 3,
            TimeSpan? delay = null,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAllWithConsistencyAsync)} was cancelled before execution");
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
                    $"{nameof(ProjectionReader<TProjection>)}.{nameof(ReadAllWithConsistencyAsync)}",
                    retryCount);
            }

            return result ?? await ReadAllAsync(cancellationToken);
        }
    }
}
