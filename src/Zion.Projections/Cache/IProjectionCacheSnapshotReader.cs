using Zion.Core.Keys;

namespace Zion.Projections.Cache
{
    public interface IProjectionCacheSnapshotReader
    {
        ValueTask<TProjection[]> GetAllAsync<TProjection>(Actor actor, CancellationToken cancellationToken = default)
            where TProjection : IProjection;
        ValueTask<TProjection> GetAsync<TProjection>(Actor actor, Subject subject, CancellationToken cancellationToken = default)
            where TProjection : IProjection;
    }
}
