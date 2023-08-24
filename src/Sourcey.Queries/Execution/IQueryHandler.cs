namespace Sourcey.Queries.Execution
{
    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> RetrieveAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
