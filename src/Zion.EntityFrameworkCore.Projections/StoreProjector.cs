using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Projections.Configuration;
using Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Zion.Events;
using Zion.Events.Stores;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections
{
    public sealed class StoreProjector<TProjection, TDbContextStore> : BackgroundService
        where TProjection : class, IProjection
        where TDbContextStore : DbContext, IEventStoreDbContext
    {
        private readonly IProjectionManager<TProjection> _projectionManager;
        private readonly IProjectionStateManager<TProjection> _projectionStateManager;
        private readonly IServiceScope _scope;
        private readonly IEventStore<TDbContextStore> _eventStore;
        private readonly ILogger<StoreProjector<TProjection, TDbContextStore>> _logger;
        private readonly IOptionsMonitor<StoreProjectorOptions<TProjection>> _options;
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;
        private readonly string _name;
        private int _retries;

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
            _projectionStateManager = _scope.ServiceProvider.GetRequiredService<IProjectionStateManager<TProjection>>();
            _eventStore = _scope.ServiceProvider.GetRequiredService<IEventStore<TDbContextStore>>();
            _options = _scope.ServiceProvider.GetRequiredService<IOptionsMonitor<StoreProjectorOptions<TProjection>>>();
            _projectionDbContextFactory = _scope.ServiceProvider.GetRequiredService<IProjectionDbContextFactory>();

            _name = typeof(TProjection).FriendlyFullName();
        }

        public async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(StoreProjector<TProjection, TDbContextStore>)}.{nameof(ResetAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await Task.WhenAll(
                _projectionManager.ResetAsync(cancellationToken),
                _projectionStateManager.RemoveAsync(cancellationToken));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(StoreProjector<TProjection, TDbContextStore>)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var state = await _projectionStateManager.RetrieveAsync(cancellationToken) ?? await _projectionStateManager.CreateAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    state = await _projectionStateManager.RetrieveAsync(cancellationToken);

                    var page = await _eventStore.GetEventsAsync(state.Position, _options.CurrentValue.PageSize, cancellationToken);;

                    await Task.WhenAll(page.Events.Select(e => ProjectStreamAsync(e.Value, cancellationToken)));

                    if (state.Position == page.Offset)
                    {
                        await Task.Delay(_options.CurrentValue.Interval, cancellationToken);
                    }
                    else
                    {
                        await _projectionStateManager.UpdateAsync(state =>
                        {
                            state.Position = page.Offset;
                            state.LastModifiedDate = DateTimeOffset.UtcNow;
                            state.Error = "";
                            state.ErrorStackTrace = "";
                        }, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Process '{Process}' failed at postion '{Position}' due to an unexpected error. See exception details for more information.", typeof(TProjection).Name, state?.Position ?? 0);

                    if(_retries++ > _options.CurrentValue.RetryCount)
                    {
                        _logger.LogCritical("Process '{Process}' stopped executed due to hitting maximum retries of: {Retries}", typeof(TProjection).Name, _options.CurrentValue.RetryCount);

                        await _projectionStateManager.UpdateAsync(state =>
                        {
                            state.Error = ex.Message;
                            state.ErrorStackTrace = ex.StackTrace;
                        }, cancellationToken);

                        break;
                    }
                }
            }
        }

        private async Task ProjectStreamAsync(IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default)
        {
            foreach (var @event in events)
                await _projectionManager.HandleAsync(@event.Payload, cancellationToken);
        }
    }
}
