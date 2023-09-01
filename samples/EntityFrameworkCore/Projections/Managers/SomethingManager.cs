using EntityFrameworkCore.Events;
using Sourcey.Projections;

namespace EntityFrameworkCore.Projections.Managers;

internal sealed class SomethingManager : ProjectionManager<Something>
{
    public SomethingManager(
        ILogger<SomethingManager> logger,
        IEnumerable<IProjectionWriter<Something>> projectionWriters,
        IEnumerable<IProjectionStateManager<Something>> projectionStateManagers) 
        : base(logger, projectionWriters, projectionStateManagers)
    {
        Handle<SomethingHappened>(OnSomethingHappenedAsync);
    }

    private async Task OnSomethingHappenedAsync(SomethingHappened @event, CancellationToken cancellationToken)
        => await AddAsync(@event.StreamId, () => new Something
        {
            Subject = @event.StreamId,
            Version = @event.Version.GetValueOrDefault(),
            Value = @event.Something
        }, cancellationToken);
}