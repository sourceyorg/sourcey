using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Projections.Initializers;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    internal readonly struct ZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> : IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext>
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        public readonly IServiceCollection _services;

        public ZionEntityFrameworkCoreProjectionsBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }
        
        public IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> For<TProjection>(Action<IZionEntityFrameworkCoreProjection<TProjection>> configuration)
            where TProjection : class, IProjection
        {
            _services.AddHostedService<StoreProjector<TProjection, TEventStoreContext>>();

            var zionEntityFrameworkCoreProjectionBuilder = new ZionEntityFrameworkCoreProjection<TProjection>();
            
            configuration(zionEntityFrameworkCoreProjectionBuilder);

            _services.Configure(zionEntityFrameworkCoreProjectionBuilder.BuildOptions());

            return this;
        }
    }
}
