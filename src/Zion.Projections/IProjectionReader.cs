using Zion.Core.Keys;

namespace Zion.Projections
{
    public interface IProjectionReader<TProjection>
        where TProjection : class, IProjection
    {
        Task<TProjection?> RetrieveAsync(Subject subject, CancellationToken cancellationToken = default);
    }
}
