namespace Sourcey.Queries.Stores
{
    public interface IQueryStore<TQueryStoreContext>
    {
        Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
    }
}
