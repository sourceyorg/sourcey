using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    internal class ZionEntityFrameworkCoreProjection<TProjection> : IZionEntityFrameworkCoreProjection<TProjection>
        where TProjection : class, IProjection
    {
        private int? _interval;
        private int? _pageSize;

        public IZionEntityFrameworkCoreProjection<TProjection> WithInterval(int interval)
        {
            _interval = interval;
            return this;
        }

        public IZionEntityFrameworkCoreProjection<TProjection> WithPageSize(int pageSize)
        {
            _interval = pageSize;
            return this;
        }

        internal Action<StoreProjectorOptions<TProjection>> BuildOptions()
            => o => 
            {
                o.Interval = _interval ?? StoreProjectorOptions<TProjection>.Default.Interval;
                o.PageSize = _pageSize ?? StoreProjectorOptions<TProjection>.Default.PageSize;
            };
    }
}
