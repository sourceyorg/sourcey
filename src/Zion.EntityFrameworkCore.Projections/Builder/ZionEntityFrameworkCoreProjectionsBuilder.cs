using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    internal readonly struct ZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> : IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext>
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        public readonly IServiceCollection Services { get; }

        public ZionEntityFrameworkCoreProjectionsBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
        public IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> For<TProjection>(Action<StoreProjectorOptions<TProjection>>? projectorOptions)
            where TProjection : class, IProjection
        {
            Services.AddHostedService<StoreProjector<TProjection, TEventStoreContext>>();
            Services.Configure(projectorOptions ?? StoreProjectorOptions<TProjection>.Default);
            return this;
        }
}
}
