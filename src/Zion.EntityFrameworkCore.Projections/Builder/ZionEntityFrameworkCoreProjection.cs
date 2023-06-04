using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    internal class ZionEntityFrameworkCoreProjection<TProjection> : IZionEntityFrameworkCoreProjection<TProjection>
        where TProjection : class, IProjection
    {
        private int? _interval;
        private int? _pageSize;
        private int? _retryCount;

        public IZionEntityFrameworkCoreProjection<TProjection> WithInterval(int interval)
        {
            _interval = interval;
            return this;
        }

        public IZionEntityFrameworkCoreProjection<TProjection> WithPageSize(int pageSize)
        {
            _pageSize = pageSize;
            return this;
        }

        public IZionEntityFrameworkCoreProjection<TProjection> WithRetries(int retryCount)
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
}
