using Sourcey.EntityFrameworkCore.Projections.Configuration;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Builder;

internal class EntityFrameworkCoreProjection<TProjection> : IEntityFrameworkCoreProjection<TProjection>
    where TProjection : class, IProjection
{
    private int? _interval;
    private int? _pageSize;
    private int? _retryCount;

    public IEntityFrameworkCoreProjection<TProjection> WithInterval(int interval)
    {
        _interval = interval;
        return this;
    }

    public IEntityFrameworkCoreProjection<TProjection> WithPageSize(int pageSize)
    {
        _pageSize = pageSize;
        return this;
    }

    public IEntityFrameworkCoreProjection<TProjection> WithRetries(int retryCount)
    {
        _retryCount = retryCount;
        return this;
    }

    internal Action<StoreProjectorOptions<TProjection>> BuildOptions()
        => o => 
        {
            o.Interval = _interval ?? StoreProjectorOptions<TProjection>.Default.Interval;
            o.PageSize = _pageSize ?? StoreProjectorOptions<TProjection>.Default.PageSize;
            o.RetryCount = _retryCount ?? StoreProjectorOptions<TProjection>.Default.RetryCount;
        };
}
