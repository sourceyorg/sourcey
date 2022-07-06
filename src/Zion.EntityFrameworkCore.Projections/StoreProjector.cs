using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.EntityFrameworkCore.Projections.Entities;
using Zion.EntityFrameworkCore.Projections.Factories;
using Zion.Events.Stores;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections
{
    public sealed class StoreProjector<TProjection, TDbContextStore> : BackgroundService
        where TProjection : class, IProjection
        where TDbContextStore : DbContext, IEventStoreDbContext
    {
        private readonly IProjectionManager<TProjection> _projectionManager;
        private readonly IServiceScope _scope;
        private readonly IEventStore<TDbContextStore> _eventStore;
        private readonly ILogger<StoreProjector<TProjection, TDbContextStore>> _logger;
        private readonly IOptionsSnapshot<StoreProjectorOptions<TProjection>> _options;
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;
        private readonly string _name;

        public StoreProjector(IServiceScopeFactory serviceScopeFactory,
            ILogger<StoreProjector<TProjection, TDbContextStore>> logger)
        {
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _scope = serviceScopeFactory.CreateScope();
            _logger = logger;

            _projectionManager = _scope.ServiceProvider.GetRequiredService<IProjectionManager<TProjection>>();
            _eventStore = _scope.ServiceProvider.GetRequiredService<IEventStore<TDbContextStore>>();
            _options = _scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<StoreProjectorOptions<TProjection>>>();
            _projectionDbContextFactory = _scope.ServiceProvider.GetRequiredService<IProjectionDbContextFactory>();

            _name = typeof(TProjection).FullName;
        }

        public async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(StoreProjector<TProjection, TDbContextStore>)}.{nameof(ResetAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using (var context = _projectionDbContextFactory.Create<TProjection>())
            {
                var state = await context.ProjectionStates.FindAsync(_name);

                if (state != null)
                {
                    state.Position = 0;
                    state.LastModifiedDate = DateTimeOffset.UtcNow;

                    await context.SaveChangesAsync(cancellationToken);
                }

                await _projectionManager.ResetAsync(cancellationToken);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(StoreProjector<TProjection, TDbContextStore>)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using (var context = _projectionDbContextFactory.Create<TProjection>())
            {
                var state = await context.ProjectionStates.FindAsync(_name);

                if (state == null)
                {
                    state = new ProjectionState
                    {
                        Key = _name,
                        CreatedDate = DateTimeOffset.UtcNow,
                        Position = 1
                    };

                    context.ProjectionStates.Add(state);

                    await context.SaveChangesAsync(cancellationToken);
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await context.Entry(state).ReloadAsync();

                        var page = await _eventStore.GetEventsAsync(state.Position);

                        foreach (var @event in page.Events)
                            await _projectionManager.HandleAsync(@event.Payload);

                        if (state.Position == page.Offset)
                        {
                            await Task.Delay(_options.Value.Interval);
                        }
                        else
                        {
                            state.Position = page.Offset;
                            state.LastModifiedDate = DateTimeOffset.UtcNow;

                            await context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, $"Process '{typeof(TProjection).Name}' failed at postion '{state.Position}' due to an unexpected error. See exception details for more information.");
                        break;
                    }
                }
            }
        }
    }
}
