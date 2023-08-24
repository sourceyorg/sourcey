using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.Builder;
using Sourcey.EntityFrameworkCore.Events.DbContexts;

namespace Sourcey.Extensions
{
    public static class EntityFrameworkCoreEventStoreBuilderExtensions
    {
        public static IEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> AddAggregates<TEventStoreContext>(this IEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> builder)
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            builder.Services.AddAggregateStore<TEventStoreContext>();
            return builder;
        }
    }
}
