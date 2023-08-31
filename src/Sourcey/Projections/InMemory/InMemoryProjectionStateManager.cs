namespace Sourcey.Projections.InMemory;

public class InMemoryProjectionState : IProjectionState
{
    public string Key { get; set; }
    public long Position { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
    public string? Error { get; set; }
    public string? ErrorStackTrace { get; set; }

}

internal sealed class InMemoryProjectionStateManager<TProjection> : IProjectionStateManager<TProjection>
    where TProjection : class, IProjection, new()
{
    private InMemoryProjectionState? _state;

    public Task<IProjectionState> CreateAsync(CancellationToken cancellationToken = default)
    {
        _state = new InMemoryProjectionState
        {
            Key = typeof(TProjection).Name,
            CreatedDate = DateTimeOffset.UtcNow
        };
        return Task.FromResult<IProjectionState>(_state);
    }

    public Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        _state = null;
        return Task.CompletedTask;
    }

    public Task<IProjectionState?> RetrieveAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IProjectionState?>(_state);
    }

    public Task<IProjectionState> UpdateAsync(Action<IProjectionState> update, CancellationToken cancellationToken = default)
    {
        if (_state == null)
            throw new InvalidOperationException("Projection state has not been created");

        update(_state);
        return Task.FromResult<IProjectionState>(_state);
    }
}