namespace Zion.Queries.Execution
{
    public interface IQueryHandler<TRequest, TResult>
        where TRequest : IQuery<TResult>
    {
        Task<TResult> RetrieveAsync(TRequest query, CancellationToken cancellationToken = default);
    }
}
