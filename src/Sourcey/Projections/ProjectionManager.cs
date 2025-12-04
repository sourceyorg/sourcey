using Microsoft.Extensions.Logging;
using Sourcey.Events;

namespace Sourcey.Projections;

public abstract class ProjectionManager<TProjection>(
    ILogger<ProjectionManager<TProjection>> logger,
    IEnumerable<IProjectionWriter<TProjection>> projectionWriters,
    IEnumerable<IProjectionStateManager<TProjection>> projectionStateManagers)
    : IProjectionManager<TProjection>
    where TProjection : class, IProjection
{
    private readonly Dictionary<Type, Func<IEvent, CancellationToken, Task>> _eventHandlers = new();

    protected readonly IEnumerable<IProjectionWriter<TProjection>> _projectionWriters = projectionWriters ?? [];
    protected readonly IEnumerable<IProjectionStateManager<TProjection>> _projectionStateManagers = projectionStateManagers ?? [];
    protected readonly ILogger<ProjectionManager<TProjection>> _logger = logger;

    protected void Handle<TEvent>(Func<TEvent, CancellationToken, Task> func)
        where TEvent : IEvent => _eventHandlers.Add(typeof(TEvent), (@event, cancellationToken) => func((TEvent)@event, cancellationToken));

    public async Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ProjectionManager<TProjection>)}.{nameof(HandleAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var type = @event.GetType();

        if (!_eventHandlers.TryGetValue(type, out var handler))
        {
            _logger.LogInformation($"Could not find handler for event type of '{type.Name}'");
            return;
        }                

        
        await handler(@event, cancellationToken);
    }

    protected virtual async Task AddAsync(string subject, Func<TProjection> add, CancellationToken cancellationToken = default)
        => await Task.WhenAll(_projectionWriters.Select(pw => pw.AddAsync(subject, add, cancellationToken)));
    protected virtual async Task AddOrUpdateAsync(string subject, Action<TProjection> update, Func<TProjection> create, CancellationToken cancellationToken = default)
        => await Task.WhenAll(_projectionWriters.Select(pw => pw.AddOrUpdateAsync(subject, update, create, cancellationToken)));
    protected virtual async Task UpdateAsync(string subject, Func<TProjection, TProjection> update, CancellationToken cancellationToken = default)
        => await Task.WhenAll(_projectionWriters.Select(pw => pw.UpdateAsync(subject, update, cancellationToken)));
    protected virtual async Task UpdateAsync(string subject, Action<TProjection> update, CancellationToken cancellationToken = default)
        => await Task.WhenAll(_projectionWriters.Select(pw => pw.UpdateAsync(subject, update, cancellationToken)));
    protected virtual async Task RemoveAsync(string subject, CancellationToken cancellationToken = default)
        => await Task.WhenAll(_projectionWriters.Select(pw => pw.RemoveAsync(subject, cancellationToken)));
    public virtual async Task ResetAsync(CancellationToken cancellationToken = default)
        => await Task.WhenAll(_projectionWriters.Select(pw => pw.ResetAsync(cancellationToken))
                .Concat(_projectionStateManagers.Select(psm => psm.RemoveAsync(cancellationToken))));
}
