namespace Sourcey.Projections
{
    public interface IProjectionWriter<TProjection>
        where TProjection : class, IProjection
    {
        Task<TProjection> AddAsync(string subject, Func<TProjection> add, CancellationToken cancellationToken = default);
        Task<TProjection> UpdateAsync(string subject, Func<TProjection, TProjection> update, CancellationToken cancellationToken = default);
        Task<TProjection> UpdateAsync(string subject, Action<TProjection> update, CancellationToken cancellationToken = default);
        Task<TProjection> AddOrUpdateAsync(string subject, Action<TProjection> update, Func<TProjection> create, CancellationToken cancellationToken = default);
        Task RemoveAsync(string subject, CancellationToken cancellationToken = default);
        Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
