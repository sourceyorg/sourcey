using Microsoft.Extensions.Options;
using Zion.AWS.SQS.Factories;
using Zion.AWS.SQS.Messages;
using Zion.Events;
using Zion.Events.Bus;

namespace Zion.AWS.SQS
{
    internal class EventBusPublisher : IEventBusPublisher
    {
        private readonly IOptionsMonitor<SQSOptions> _optionsMonitor;
        private readonly IClientFactory _clientFactory;
        private readonly IMessageFactory _messageFactory;

        public EventBusPublisher(
            IOptionsMonitor<SQSOptions> optionsMonitor,
            IClientFactory clientFactory,
            IMessageFactory messageFactory)
        {
            if (optionsMonitor is null)
                throw new ArgumentNullException(nameof(optionsMonitor));
            if (clientFactory is null)
                throw new ArgumentNullException(nameof(clientFactory));
            if (messageFactory is null)
                throw new ArgumentNullException(nameof(messageFactory));

            _optionsMonitor = optionsMonitor;
            _clientFactory = clientFactory;
            _messageFactory = messageFactory;
        }

        public async Task PublishAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default)
            where TEvent : IEvent
        {
            var messages = _messageFactory.CreateMessages(_optionsMonitor.CurrentValue.SQSPublishQueues, context, cancellationToken);

            using var client = _clientFactory.Create();
            await Task.WhenAll(messages.Select(m => client.SendMessageAsync(m, cancellationToken)));
        }

        public async Task PublishAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            var messages = _messageFactory.CreateBatchMessages(_optionsMonitor.CurrentValue.SQSPublishQueues, contexts, cancellationToken);

            using var client = _clientFactory.Create();
            await Task.WhenAll(messages.Select(m => client.SendMessageBatchAsync(m, cancellationToken)));
        }
    }
}
