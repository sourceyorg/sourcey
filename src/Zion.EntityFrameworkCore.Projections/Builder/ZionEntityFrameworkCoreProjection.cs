using Microsoft.Extensions.DependencyInjection;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    internal class ZionEntityFrameworkCoreProjection<TProjection> : IZionEntityFrameworkCoreProjection<TProjection>
        where TProjection : class, IProjection
    {
        private readonly IServiceCollection _services;

        private int? _interval;
        private bool _hasManagerSet;

        public ZionEntityFrameworkCoreProjection(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

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

        internal void Validate()
        {
            if(!_hasManagerSet)
                throw new InvalidOperationException($"You must set a ProjectionStateDbContext for {typeof(TProjection).FriendlyFullName()}");
        }
    }
}
