using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Events.Builder;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Events.Factories;
using Zion.EntityFrameworkCore.Events.Initializers;
using Zion.EntityFrameworkCore.Events.Stores;
using Zion.Events.Stores;

namespace Zion.EntityFrameworkCore.Events.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> AddEventStore<TEventStoreContext>(
            this IZionEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            bool autoMigrate = true)
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            builder.Services.AddDbContext<TEventStoreContext>(options);
            builder.Services.TryAdd(GetStoreServices<TEventStoreContext>());
            builder.Services.TryAdd(GetFactoryServices());
            builder.Services.AddScoped<IZionInitializer, EventStoreInitializer<TEventStoreContext>>();
            builder.Services.AddSingleton(new EventStoreInitializerOptions<TEventStoreContext>(autoMigrate));

            return new ZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext>(builder);
        }

        private static IEnumerable<ServiceDescriptor> GetStoreServices<TEventStoreContext>()
            where TEventStoreContext : DbContext, IEventStoreDbContext
        {
            yield return ServiceDescriptor.Scoped<IEventStore<TEventStoreContext>, EventStore<TEventStoreContext>>();
            yield return ServiceDescriptor.Scoped<IEventStoreDbContextFactory<TEventStoreContext>, EventStoreDbContextFactory<TEventStoreContext>>();
        }

        private static IEnumerable<ServiceDescriptor> GetFactoryServices()
        {
            yield return ServiceDescriptor.Scoped<IEventContextFactory, EventContextFactory>();
            yield return ServiceDescriptor.Scoped<IEventModelFactory, EventModelFactory>();
        }
    }
}
