using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Aggregates.Stores;
using Zion.EntityFrameworkCore.Events.Builder;
using Zion.EntityFrameworkCore.Events.DbContexts;

namespace Zion.EntityFrameworkCore.Aggregates.Extensions
{
    public static class ZionEntityFrameworkCoreEventStoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> AddAggregates<TEventStoreContext>(this IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> builder)
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            builder.Services.TryAdd(GetAggregateServices<TEventStoreContext>());
            return builder;
        }

        private static IEnumerable<ServiceDescriptor> GetAggregateServices<TEventStoreContext>()
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            yield return ServiceDescriptor.Scoped<IAggregateStore<TEventStoreContext>, AggregateStore<TEventStoreContext>>();
        }
    }
}
