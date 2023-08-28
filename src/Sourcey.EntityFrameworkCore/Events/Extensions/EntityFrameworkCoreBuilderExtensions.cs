using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Initialization;
using Sourcey.EntityFrameworkCore.Builder;
using Sourcey.EntityFrameworkCore.Events.Builder;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.EntityFrameworkCore.Events.Factories;
using Sourcey.EntityFrameworkCore.Events.Initializers;
using Sourcey.EntityFrameworkCore.Events.Stores;
using Sourcey.Events.Stores;

namespace Sourcey.Extensions;

public static class EntityFrameworkCoreBuilderExtensions
{
    public static IEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> AddEventStore<TEventStoreContext>(
        this IEntityFrameworkCoreBuilder builder,
        Action<DbContextOptionsBuilder> options,
        bool autoMigrate = true)
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        builder.Services.AddDbContext<TEventStoreContext>(options);
        builder.Services.TryAdd(GetStoreServices<TEventStoreContext>());
        builder.Services.TryAdd(GetFactoryServices());
        builder.Services.AddScoped<ISourceyInitializer, EventStoreInitializer<TEventStoreContext>>();
        builder.Services.AddSingleton(new EventStoreInitializerOptions<TEventStoreContext>(autoMigrate));

        return new EntityFrameworkCoreEventStoreBuilder<TEventStoreContext>(builder);
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
