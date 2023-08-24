namespace Sourcey.Queries.Execution
{
    public interface IPostQueryMiddleware<TQuery, TResult> : IQueryHandlerMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}
