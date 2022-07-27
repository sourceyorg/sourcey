﻿using Zion.Core.Stores;

namespace Zion.Queries.Stores
{
    public abstract class BufferedQueryStore : BufferedStore<IQuery<object>>, IQueryStore
    {
        public Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
            => InternalSaveAsync((IQuery<object>)query, cancellationToken);
    }
}
