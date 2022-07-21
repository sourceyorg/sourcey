using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Events.Factories;

namespace Zion.EntityFrameworkCore.Events.Initializers
{
    internal class EventStoreInitializer<TEventStoreDbContext> : IZionInitializer
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
            if (!_options._autoMigrate)
                return;
            
            using var context = _eventStoreDbContextFactory.Create();
            await context.Database.EnsureCreatedAsync();
            await context.Database.MigrateAsync();
        }
    }
}
