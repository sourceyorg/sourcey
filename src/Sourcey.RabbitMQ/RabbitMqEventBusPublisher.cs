using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sourcey.Events;
using Sourcey.Events.Bus;
using Sourcey.RabbitMQ.Queues;

namespace Sourcey.RabbitMQ
{
    internal sealed class RabbitMqEventBusPublisher : IEventBusPublisher
    {
        private readonly ILogger<RabbitMqEventBusPublisher> _logger;
        private readonly IQueueMessageSender _queueMessageSender;

        public RabbitMqEventBusPublisher(ILogger<RabbitMqEventBusPublisher> logger,
                                IQueueMessageSender queueMessageSender)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (queueMessageSender == null)
                throw new ArgumentNullException(nameof(queueMessageSender));

            _logger = logger;
            _queueMessageSender = queueMessageSender;
        }

        public async Task PublishAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            cancellationToken.ThrowIfCancellationRequested();

            await _queueMessageSender.SendAsync(context, cancellationToken);
        }

        public async Task PublishAsync(IEnumerable<IEventNotification<IEvent>> events, CancellationToken cancellationToken = default)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            cancellationToken.ThrowIfCancellationRequested();

            await _queueMessageSender.SendAsync(events, cancellationToken);
        }
    }
}
