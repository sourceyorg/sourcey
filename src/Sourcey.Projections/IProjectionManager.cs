using Sourcey.Events;

namespace Sourcey.Projections
{
    public interface IProjectionManager<TProjection>
        where TProjection : class, IProjection
    {
        Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default);
        Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
