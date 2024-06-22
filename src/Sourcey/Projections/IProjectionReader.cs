using System.Linq.Expressions;
using Sourcey.Keys;

namespace Sourcey.Projections;

public interface IProjectionReader<TProjection>
    where TProjection : class, IProjection
{
    ValueTask<TProjection?> ReadAsync(
        Subject subject,
        CancellationToken cancellationToken = default);

    ValueTask<TResult?> ReadAsync<TResult>(
        Subject subject,
        Expression<Func<TProjection, TResult>> projection,
        CancellationToken cancellationToken = default);

    ValueTask<TProjection?> ReadWithConsistencyAsync(Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default);

    ValueTask<TResult?> ReadWithConsistencyAsync<TResult>(Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck,
        Expression<Func<TProjection, TResult>> projection,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default);

    ValueTask<IQueryableProjection<TProjection>> QueryAsync(CancellationToken cancellationToken = default);

    ValueTask<IQueryableProjection<TProjection>> QueryWithConsistencyAsync(
        Subject subject,
        Expression<Func<TProjection?, bool>> consistencyCheck,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default);

    ValueTask<IQueryableProjection<TProjection>> QueryWithConsistencyAsync(
        Func<IQueryable<TProjection>, ValueTask<bool>> consistencyCheckAsync,
        int retryCount = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default);
}
