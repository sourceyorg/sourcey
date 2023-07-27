using Zion.Core.Keys;

namespace Zion.Projections
{
    public interface IProjectionReader<TProjection>
        where TProjection : class, IProjection
    {
        ValueTask<TProjection?> ReadAsync(Subject subject, CancellationToken cancellationToken = default);
        ValueTask<TProjection?> ReadWithConsistencyAsync(Subject subject, Func<TProjection?, bool> consistencyCheck, int retryCount = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default);
        ValueTask<IQueryableProjection<TProjection>> ReadAllAsync(CancellationToken cancellationToken = default);
        ValueTask<IQueryableProjection<TProjection>> ReadAllWithConsistencyAsync(Subject subject, Func<TProjection?, bool> consistencyCheck, int retryCount = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default);
        ValueTask<IQueryableProjection<TProjection>> ReadAllWithConsistencyAsync(Func<IQueryable<TProjection>, ValueTask<bool>> consistencyCheckAsync, int retryCount = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default);
    }
}
