using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zion.Core.Extensions;
using Zion.Queries.Stores;

namespace Zion.Queries.Execution
{
    internal sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly ILogger<QueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(ILogger<QueryDispatcher> logger,
            IServiceProvider serviceProvider)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<TQueryResult?> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            cancellationToken.ThrowIfCancellationRequested();

            var type = query.GetType();

            _logger.LogInformation($"Dispatching query '{type.FriendlyName()}' returning '{typeof(TQueryResult).FriendlyName()}'.");

            var handler = _serviceProvider.GetService(typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TQueryResult)));

            if (handler == null)
                throw new InvalidOperationException($"No query handler for type '{type.FriendlyName()}' has been registered.");

            if (!await DispatchMiddleWare(type, typeof(IPreQueryMiddleware<,>), query, cancellationToken))
                return default;

            var result = await (Task<TQueryResult>)handler.GetType().GetMethod("RetrieveAsync").Invoke(handler, new object[] { query, cancellationToken });

            await DispatchMiddleWare(type, typeof(IPostQueryMiddleware<,>), query, cancellationToken);

            return result;
        }

        private async Task<bool> DispatchMiddleWare<TQueryResult>(Type queryType, Type middleWareType, IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
        {
            var middlewares = _serviceProvider.GetServices(middleWareType.MakeGenericType(queryType, typeof(TQueryResult)));

            if (middlewares is null)
                return true;
            
            foreach (var middleware in middlewares)
            {
                var result = await (Task<bool>)middleware.GetType().GetMethod("ExecuteAsync").Invoke(middleware, new object[] { query, cancellationToken });

                if (!result)
                    return false;
            }

            return true;
        }
    }
}
