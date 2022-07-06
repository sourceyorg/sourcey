namespace Zion.Queries.Stores
{
    internal sealed class NoOpQueryStore : IQueryStore
    {
        public Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
