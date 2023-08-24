using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Builder
{
    public interface IEntityFrameworkCoreProjection<TProjection>
        where TProjection : class, IProjection
    {
        IEntityFrameworkCoreProjection<TProjection> WithInterval(int interval);
        IEntityFrameworkCoreProjection<TProjection> WithPageSize(int pageSize);
        IEntityFrameworkCoreProjection<TProjection> WithRetries(int retryCount);
    }
}
