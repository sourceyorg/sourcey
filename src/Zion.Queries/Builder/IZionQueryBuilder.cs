using Zion.Queries.Execution;

namespace Zion.Queries.Builder
{
    public interface IZionQueryBuilder<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        IZionQueryBuilder<TQuery, TResult> WithHandler<THandler>()
            where THandler : class, IQueryHandler<TQuery, TResult>;
        IZionQueryBuilder<TQuery, TResult> BeforeHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>;
        IZionQueryBuilder<TQuery, TResult> AfterHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>;
        IZionQueryBuilder<TQuery, TResult> WithQueryStoreLogging();
    }
}
