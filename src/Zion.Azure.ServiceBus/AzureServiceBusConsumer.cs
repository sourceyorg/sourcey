using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zion.Azure.ServiceBus.Subscriptions;
using Zion.Events.Bus;

namespace Zion.Azure.ServiceBus
{
    internal sealed class AzureServiceBusConsumer : IEventBusConsumer
    {
        private readonly ILogger<AzureServiceBusConsumer> _logger;
        private readonly ISubscriptionClientManager _subscriptionClientManager;
        private bool _running = false;
        private IReadOnlyList<ISubscriptionClient>? _clients;

        public AzureServiceBusConsumer(ILogger<AzureServiceBusConsumer> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _logger = logger;

            _subscriptionClientManager = serviceScopeFactory
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ISubscriptionClientManager>();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_running)
                return;

            _logger.LogInformation($"Starting Azure Service bus");

            _clients = await _subscriptionClientManager.RegisterClientsAsync();

            _running = true;

            _logger.LogInformation($"Successfully started Azure Service bus");
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_running)
                return;

            _logger.LogInformation($"Stopping Azure Service bus");

            if(_clients?.Any() == true)
                foreach (var client in _clients)
                    await client.CloseAsync();

            _running = false;

            _logger.LogInformation($"Successfully stopped Azure Service bus");
        }
    }
}
