using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.Builder;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Builder;

namespace Sourcey.Extensions
{
    public static class EntityFrameworkCoreEventStoreBuilderExtensions
    {
        public static IEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> AddProjections<TEventStoreContext>(this IEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> builder,
            Action<IEntityFrameworkCoreProjectionsBuilder<TEventStoreContext>> configuration)
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            var entityFrameworkCoreProjectionsBuilder = new EntityFrameworkCoreProjectionsBuilder<TEventStoreContext>(builder.Services);
            configuration(entityFrameworkCoreProjectionsBuilder);
            return builder;
        }
}
}
