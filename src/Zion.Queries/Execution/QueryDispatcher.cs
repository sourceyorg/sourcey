using Microsoft.Extensions.Logging;
using Zion.Core.Extensions;
using Zion.Queries.Stores;

namespace Zion.Queries.Execution
{
    internal sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly ILogger<QueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IQueryStore _queryStore;

        public QueryDispatcher(ILogger<QueryDispatcher> logger,
            IServiceProvider serviceProvider,
            IQueryStore queryStore)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (queryStore == null)
                throw new ArgumentNullException(nameof(queryStore));

            _logger = logger;
            _serviceProvider = serviceProvider;
            _queryStore = queryStore;
        }

        public async Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            cancellationToken.ThrowIfCancellationRequested();

            var type = query.GetType();

            _logger.LogInformation($"Dispatching query '{type.FriendlyName()}' returning '{typeof(TQueryResult).FriendlyName()}'.");

            var handler = _serviceProvider.GetService(typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TQueryResult)));

            if (handler == null)
                throw new InvalidOperationException($"No query handler for type '{type.FriendlyName()}' has been registered.");

            var result = await (Task<TQueryResult>)handler.GetType().GetMethod("RetrieveAsync").Invoke(handler, new object[] { query, cancellationToken });

            await _queryStore.SaveAsync<TQueryResult>(query, cancellationToken);

            return result;
        }
    }
}
