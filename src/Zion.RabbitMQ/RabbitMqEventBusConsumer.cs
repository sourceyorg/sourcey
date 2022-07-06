using Microsoft.Extensions.Logging;
using Zion.Events.Bus;
using Zion.RabbitMQ.Queues;

namespace Zion.RabbitMQ
{
    internal sealed class RabbitMqEventBusConsumer : IEventBusConsumer
    {
        private readonly ILogger<RabbitMqEventBusConsumer> _logger;
        private readonly IQueueMessageReceiver _queueMessageReceiver;

        public RabbitMqEventBusConsumer(ILogger<RabbitMqEventBusConsumer> logger,
                                IQueueMessageReceiver queueMessageReceiver)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (queueMessageReceiver == null)
                throw new ArgumentNullException(nameof(queueMessageReceiver));

            _logger = logger;
            _queueMessageReceiver = queueMessageReceiver;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            //if (_running)
            //    return;

            _logger.LogInformation($"Starting RabbitMQ bus");


            await _queueMessageReceiver.StartAsync(cancellationToken);


            //_running = true;

            _logger.LogInformation($"Successfully started RabbitMQ bus");
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            //if (!_running)
            //    return;

            _logger.LogInformation($"Stopping RabbitMQ bus");

            await _queueMessageReceiver.StopAsync(cancellationToken);

            //_running = false;

            _logger.LogInformation($"Successfully stopped RabbitMQ bus");
        }
    }
}
