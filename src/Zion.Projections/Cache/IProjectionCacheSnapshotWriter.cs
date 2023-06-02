using Zion.Core.Keys;
using Zion.Events;

namespace Zion.Projections.Cache
{
    public interface IProjectionCacheSnapshotWriter
    {
        ValueTask WriteAsync(Actor actor, IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
    }
}
