using Sourcey.Queries.Stores;

namespace Sourcey.Queries.Execution
{
    internal class QueryStoreMiddleware<TQuery, TResult, TQueryStoreContext> : IPostQueryMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryStore<TQueryStoreContext> _queryStore;

        public QueryStoreMiddleware(IQueryStore<TQueryStoreContext> queryStore)
        {
            if (queryStore is null)
                throw new ArgumentNullException(nameof(queryStore));

            _queryStore = queryStore;
        }

        public async Task<bool> ExecuteAsync(TQuery query, CancellationToken cancellationToken = default)
        {
            await _queryStore.SaveAsync(query, cancellationToken);
            return true;
        }
    }
}
