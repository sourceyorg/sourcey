namespace Zion.Queries.Execution
{
    public interface IQueryDispatcher
    {
        Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
    }
}
