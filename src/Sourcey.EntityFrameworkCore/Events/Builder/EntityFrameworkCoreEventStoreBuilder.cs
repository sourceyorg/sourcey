using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.EntityFrameworkCore.Events.Factories;
using Sourcey.EntityFrameworkCore.Events.Initializers;
using Sourcey.EntityFrameworkCore.Events.Stores;
using Sourcey.Events.Builder;
using Sourcey.Events.Stores;
using Sourcey.Initialization;

namespace Sourcey.EntityFrameworkCore.Events.Builder;

internal sealed class EntityFrameworkCoreEventStoreBuilder<TEventStoreContext> : BaseEventStoreBuilder<TEventStoreContext>
    where TEventStoreContext : DbContext, IEventStoreDbContext
{
    public EntityFrameworkCoreEventStoreBuilder(
        IServiceCollection services,
        bool autoMigrate = true) : base(services)
    {
        services.AddScoped<ISourceyInitializer, EventStoreInitializer<TEventStoreContext>>();
        services.AddSingleton(new EventStoreInitializerOptions<TEventStoreContext>(autoMigrate));
        services.TryAddScoped<IEventContextFactory, EventContextFactory>();
        services.TryAddScoped<IEventModelFactory, EventModelFactory>();
        services.AddScoped<IEventStore<TEventStoreContext>, EventStore<TEventStoreContext>>();
    }

    protected override TEventStoreContext GetEventStoreContext(IServiceProvider provider)
    {
        var factory = provider.GetRequiredService<IDbContextFactory<TEventStoreContext>>();
        return factory.CreateDbContext();
    }
}
