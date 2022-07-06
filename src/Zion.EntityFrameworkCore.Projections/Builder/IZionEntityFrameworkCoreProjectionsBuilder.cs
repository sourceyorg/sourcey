using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    public interface IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> 
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> For<TProjection>(Action<StoreProjectorOptions<TProjection>>? projectorOptions)
            where TProjection : class, IProjection;
    }
}
