using Microsoft.Extensions.Logging;
using Sourcey.Azure.ServiceBus.Topics;
using Sourcey.Events;
using Sourcey.Events.Bus;

namespace Sourcey.Azure.ServiceBus
{
    internal sealed class AzureServiceBusPublisher : IEventBusPublisher
    {
        private readonly ILogger<AzureServiceBusPublisher> _logger;
        private readonly ITopicMessageSender _messageSender;

        public AzureServiceBusPublisher(ILogger<AzureServiceBusPublisher> logger,
                               ITopicMessageSender messageSender)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (messageSender == null)
                throw new ArgumentNullException(nameof(messageSender));

            _logger = logger;
            _messageSender = messageSender;
        }

        public async Task PublishAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            await _messageSender.SendAsync(context, cancellationToken);
        }
        public async Task PublishAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));

            await _messageSender.SendAsync(contexts, cancellationToken);
        }
    }
}
