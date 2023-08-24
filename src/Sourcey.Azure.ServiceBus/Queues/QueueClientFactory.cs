using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Queues
{
    internal sealed class QueueClientFactory : IQueueClientFactory
    {
        private readonly IDictionary<string, ServiceBusQueueOptions> _options;

        public QueueClientFactory(IEnumerable<ServiceBusQueueOptions> options)
        {
            _options = (options ?? Enumerable.Empty<ServiceBusQueueOptions>())
                .ToDictionary(o => o.EntityPath);
        }

        public IQueueClient Create(ServiceBusQueueOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var client = new QueueClient(
                connectionString: options.ConnectionString,
                entityPath: options.EntityPath,
                receiveMode: options.ReceiveMode,
                retryPolicy: options.RetryPolicy);

            return client;
        }

        public IQueueClient Create(string queue)
        {
            if (!_options.TryGetValue(queue, out var options))
                throw new ArgumentNullException(nameof(queue));

            return Create(options);
        }
    }
}
