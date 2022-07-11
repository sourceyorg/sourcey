using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    public interface IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> 
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> For<TProjection>(Action<IZionEntityFrameworkCoreProjection<TProjection>> configuration)
            where TProjection : class, IProjection;
    }
}
