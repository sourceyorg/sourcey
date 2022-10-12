using Zion.Core.Stores;

namespace Zion.Queries.Stores
{
    public abstract class BufferedQueryStore<TQueryStoreContext> : BufferedStore<IQuery<object>>, IQueryStore<TQueryStoreContext>
    {
        public Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
            => InternalSaveAsync((IQuery<object>)query, cancellationToken);
    }
}
