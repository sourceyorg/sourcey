using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    internal class ZionEntityFrameworkCoreProjection<TProjection> : IZionEntityFrameworkCoreProjection<TProjection>
        where TProjection : class, IProjection
    {
        private int? _interval;

        public IZionEntityFrameworkCoreProjection<TProjection> WithInterval(int interval)
        {
            _interval = interval;
            return this;
        }
        
        internal Action<StoreProjectorOptions<TProjection>> BuildOptions()
            => o => 
            {
                o.Interval = _interval ?? StoreProjectorOptions<TProjection>.Default.Interval;
            };
    }
}
