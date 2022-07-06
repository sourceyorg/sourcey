namespace Zion.Queries.Stores
{
    public interface IQueryStore
    {
        Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
    }
}
