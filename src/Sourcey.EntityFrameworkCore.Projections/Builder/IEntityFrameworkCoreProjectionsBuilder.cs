using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Builder
{
    public interface IEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> 
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        IEntityFrameworkCoreProjectionsBuilder<TEventStoreContext> For<TProjection>(Action<IEntityFrameworkCoreProjection<TProjection>> configuration)
            where TProjection : class, IProjection;
    }
}
