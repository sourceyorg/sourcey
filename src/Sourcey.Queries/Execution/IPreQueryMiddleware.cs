namespace Sourcey.Queries.Execution
{
    public interface IPreQueryMiddleware<TQuery, TResult> : IQueryHandlerMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}
