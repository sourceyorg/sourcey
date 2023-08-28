using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Builder;

internal readonly struct EntityFrameworkCoreProjectionsBuilder<TEventStoreContext> : IEntityFrameworkCoreProjectionsBuilder<TEventStoreContext>
    where TEventStoreContext : DbContext, IEventStoreDbContext
{
    public readonly IServiceCollection _services;

    public EntityFrameworkCoreProjectionsBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }
    
    public IEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> For<TProjection>(Action<IEntityFrameworkCoreProjection<TProjection>> configuration)
        where TProjection : class, IProjection
    {
        _services.AddHostedService<StoreProjector<TProjection, TEventStoreContext>>();

        var sourceyEntityFrameworkCoreProjectionBuilder = new EntityFrameworkCoreProjection<TProjection>();
        
        configuration(sourceyEntityFrameworkCoreProjectionBuilder);

        _services.Configure(sourceyEntityFrameworkCoreProjectionBuilder.BuildOptions());

        return this;
    }
}
