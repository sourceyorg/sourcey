using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Sourcey.Initialization;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.EntityFrameworkCore.Events.Factories;

namespace Sourcey.EntityFrameworkCore.Events.Initializers;

internal class EventStoreInitializer<TEventStoreDbContext> : ISourceyInitializer
    where TEventStoreDbContext : DbContext, IEventStoreDbContext
{
    public bool ParallelEnabled => false;
    private readonly IEventStoreDbContextFactory<TEventStoreDbContext> _eventStoreDbContextFactory;
    private readonly EventStoreInitializerOptions<TEventStoreDbContext> _options;


    public EventStoreInitializer(IEventStoreDbContextFactory<TEventStoreDbContext> eventStoreDbContextFactory,
        EventStoreInitializerOptions<TEventStoreDbContext> options)
    {
        if (eventStoreDbContextFactory is null)
            throw new ArgumentNullException(nameof(eventStoreDbContextFactory));
        if(options is null)
            throw new ArgumentNullException(nameof(options));

        _eventStoreDbContextFactory = eventStoreDbContextFactory;
        _options = options;
    }

    public async Task InitializeAsync(IHost host)
    {
        if (!_options.AutoMigrate)
            return;
        
        using var context = _eventStoreDbContextFactory.Create();
        await context.Database.MigrateAsync();
    }
}
