using Microsoft.EntityFrameworkCore;
using Zion.Aggregates.Extensions;
using Zion.EntityFrameworkCore.Events.Builder;
using Zion.EntityFrameworkCore.Events.DbContexts;

namespace Zion.EntityFrameworkCore.Extensions
{
    public static class ZionEntityFrameworkCoreEventStoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> AddAggregates<TEventStoreContext>(this IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> builder)
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            builder.Services.AddAggregateStore<TEventStoreContext>();
            return builder;
        }
    }
}
