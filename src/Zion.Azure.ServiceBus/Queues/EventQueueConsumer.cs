using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zion.Events.Bus;

namespace Zion.Azure.ServiceBus.Queues
{
    internal sealed class EventQueueConsumer : IEventQueueConsumer
    {
        private readonly ILogger<EventQueueConsumer> _logger;
        private readonly IQueueClientManager _queueClientManager;
        private readonly string _queueName;
        private bool _running = false;
        private IQueueClient _client;

        public EventQueueConsumer(string queueName,
            ILogger<EventQueueConsumer> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            if (queueName == null)
                throw new ArgumentNullException(nameof(queueName));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _logger = logger;

            _queueClientManager = serviceScopeFactory
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<IQueueClientManager>();

            _queueName = queueName;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_running)
                return;

            _logger.LogInformation($"Starting Azure Service bus");

            _client = await _queueClientManager.RegisterClientAsync(_queueName);

            _running = true;

            _logger.LogInformation($"Successfully started Azure Service bus");
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_running)
                return;

            _logger.LogInformation($"Stopping Azure Service bus");

            if (_client is not null)
                await _client.CloseAsync();

            _running = false;

            _logger.LogInformation($"Successfully stopped Azure Service bus");
        }
    }
}
