using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.Builder;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Projections.Builder;

namespace Zion.EntityFrameworkCore.Extensions
{
    public static class ZionEntityFrameworkCoreEventStoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> AddProjections<TEventStoreContext>(this IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> builder,
            Action<IZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext>> configuration)
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            var entityFrameworkCoreProjectionsBuilder = new ZionEntityFrameworkCoreProjectionsBuilder<TEventStoreContext>(builder.Services);
            configuration(entityFrameworkCoreProjectionsBuilder);
            return builder;
        }
}
}
