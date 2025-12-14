using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sourcey.Initialization;
using Sourcey.EntityFrameworkCore.Events.DbContexts;

namespace Sourcey.EntityFrameworkCore.Events.Initializers;

internal class EventStoreInitializer<TEventStoreDbContext> : ISourceyInitializer
    where TEventStoreDbContext : DbContext, IEventStoreDbContext
{
    public bool ParallelEnabled => false;
    private readonly IDbContextFactory<TEventStoreDbContext> _eventStoreDbContextFactory;
    private readonly EventStoreInitializerOptions<TEventStoreDbContext> _options;
    private readonly ILogger<EventStoreInitializerOptions<TEventStoreDbContext>> _logger;


    public EventStoreInitializer(IDbContextFactory<TEventStoreDbContext> eventStoreDbContextFactory,
        EventStoreInitializerOptions<TEventStoreDbContext> options,
        ILogger<EventStoreInitializerOptions<TEventStoreDbContext>> logger)
    {
        ArgumentNullException.ThrowIfNull(eventStoreDbContextFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _eventStoreDbContextFactory = eventStoreDbContextFactory;
        _options = options;
        _logger = logger;
    }

    public async Task InitializeAsync(IHost host)
    {
        if (!_options.AutoMigrate)
            return;
        
        _logger.LogDebug("Starting - migration {context}", typeof(TEventStoreDbContext).FullName);
        var success = false;
        var attempts = 0;
        
        while (!success && attempts < 10)
        {
            try
            {
                var context = await _eventStoreDbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
                await using (context.ConfigureAwait(false))
                {
                    await context.Database.MigrateAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogDebug("Unable to migrate {context}, attempts: {attempt}, exception: {exception}", typeof(TEventStoreDbContext).FullName, attempts++, e.Message);
                await Task.Delay(200).ConfigureAwait(false);
                continue;
            }

            success = true;
        }

        _logger.LogDebug("Finished - migration {context}", typeof(TEventStoreDbContext).FullName);
    }
}
