namespace Sourcey.Queries.Execution
{
    public interface IQueryHandlerMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<bool> ExecuteAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
