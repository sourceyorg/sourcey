using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sourcey.Extensions;
using Sourcey.Events;
using Sourcey.Events.Stores;
using Sourcey.Projections.Configuration;

namespace Sourcey.Projections;

public sealed class StoreProjector<TProjection> : BackgroundService
    where TProjection : class, IProjection
{
    private readonly IProjectionManager<TProjection> _projectionManager;
    private readonly IProjectionStateManager<TProjection> _projectionStateManager;
    private readonly IServiceScope _scope;
    private readonly IEventStoreFactory _eventStoreFactory;
    private readonly ILogger<StoreProjector<TProjection>> _logger;
    private readonly StoreProjectorOptions _options;
    private readonly string _name;
    private int _retries = 0;

    public StoreProjector(IServiceScopeFactory serviceScopeFactory,
        ILogger<StoreProjector<TProjection>> logger)
    {
        if (serviceScopeFactory == null)
            throw new ArgumentNullException(nameof(serviceScopeFactory));
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        _scope = serviceScopeFactory.CreateScope();
        _logger = logger;

        _projectionManager = _scope.ServiceProvider.GetRequiredService<IProjectionManager<TProjection>>();
        _projectionStateManager = _scope.ServiceProvider.GetRequiredService<IProjectionStateManager<TProjection>>();
        _eventStoreFactory = _scope.ServiceProvider.GetRequiredService<IEventStoreFactory>();
        _name = typeof(TProjection).FriendlyFullName();
        _options = _scope.ServiceProvider.GetKeyedService<StoreProjectorOptions>(StoreProjectorOptions.GetKey(_name)) ?? new StoreProjectorOptions();

    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(StoreProjector<TProjection>)}.{nameof(ResetAsync)} was cancelled before execution");
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
            _logger.LogInformation($"{nameof(StoreProjector<TProjection>)}.{nameof(ExecuteAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var state = await _projectionStateManager.RetrieveAsync(cancellationToken) ?? await _projectionStateManager.CreateAsync(cancellationToken);
        var eventStore = _eventStoreFactory.Create<TProjection>();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                state = await _projectionStateManager.RetrieveAsync(cancellationToken) ?? await _projectionStateManager.CreateAsync(cancellationToken);;

                var page = await eventStore.GetEventsAsync(state.Position, _options.PageSize, cancellationToken);;

                await Task.WhenAll(page.Events.Select(e => ProjectStreamAsync(e.Value, cancellationToken)));

                if (state.Position == page.Offset)
                {
                    await Task.Delay(_options.Interval, cancellationToken);
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
                _retries++;
                _logger.LogCritical("Process '{process}', retries: {retries} of {max}", _name, _retries, _options.RetryCount);

                if(_retries == _options.RetryCount)
                {
                    _logger.LogCritical("Process '{Process}' stopped executed due to hitting maximum retries of: {Retries}", _name, _options.RetryCount);

                    var (error, errorStackTrace) = GetError(ex);

                    await _projectionStateManager.UpdateAsync(state =>
                    {
                        state.Error = error.ToString();
                        state.ErrorStackTrace = errorStackTrace.ToString();
                    }, cancellationToken);

                    break;
                }
            }
        }
    }

    private (StringBuilder error, StringBuilder errorStackTrace) GetError(Exception ex, StringBuilder? error = null, StringBuilder? errorStackTrace = null)
    {
        error ??= new StringBuilder();
        errorStackTrace ??= new StringBuilder();

        error.AppendLine();
        error.AppendLine(ex.Message);

        errorStackTrace.AppendLine();
        errorStackTrace.AppendLine(ex.StackTrace);

        if (ex.InnerException is not null)
        {
            return GetError(ex.InnerException, error, errorStackTrace);
        }

        return (error, errorStackTrace);
    }

    private async Task ProjectStreamAsync(IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
            await _projectionManager.HandleAsync(@event.Payload, cancellationToken);
    }
}
