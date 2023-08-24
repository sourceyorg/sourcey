using Sourcey.Queries.Execution;

namespace Sourcey.Queries.Builder
{
    public interface IQueryBuilder<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        IQueryBuilder<TQuery, TResult> WithHandler<THandler>()
            where THandler : class, IQueryHandler<TQuery, TResult>;
        IQueryBuilder<TQuery, TResult> BeforeHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>;
        IQueryBuilder<TQuery, TResult> AfterHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>;
        IQueryBuilder<TQuery, TResult> WithQueryStoreLogging<TQueryStoreContext>();
    }
}
