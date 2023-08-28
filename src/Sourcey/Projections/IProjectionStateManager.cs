namespace Sourcey.Projections;

public interface IProjectionStateManager<TProjection>
    where TProjection : class, IProjection
{
    Task<IProjectionState?> RetrieveAsync(CancellationToken cancellationToken = default);
    Task<IProjectionState> UpdateAsync(Action<IProjectionState> update, CancellationToken cancellationToken = default);
    Task<IProjectionState> CreateAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(CancellationToken cancellationToken = default);
}
