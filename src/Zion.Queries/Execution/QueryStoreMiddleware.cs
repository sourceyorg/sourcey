using Zion.Queries.Stores;

namespace Zion.Queries.Execution
{
    internal class QueryStoreMiddleware<TQuery, TResult> : IPostQueryMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryStore _queryStore;

        public QueryStoreMiddleware(IQueryStore queryStore)
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
