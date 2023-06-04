using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    public interface IZionEntityFrameworkCoreProjection<TProjection>
        where TProjection : class, IProjection
    {
        IZionEntityFrameworkCoreProjection<TProjection> WithInterval(int interval);
        IZionEntityFrameworkCoreProjection<TProjection> WithPageSize(int pageSize);
        IZionEntityFrameworkCoreProjection<TProjection> WithRetries(int retryCount);
    }
}
