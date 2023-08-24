using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Queues
{
    internal sealed class QueueClientManager : IQueueClientManager
    {
        private readonly IQueueClientFactory _queueClientFactory;
        private readonly IQueueMessageReceiver _messageReceiver;
        private IQueueClient _client;

        public QueueClientManager(IQueueClientFactory subscriptionClientFactory,
                                                IQueueMessageReceiver messageReceiver)
        {
            if (subscriptionClientFactory == null)
                throw new ArgumentNullException(nameof(subscriptionClientFactory));
            if (messageReceiver == null)
                throw new ArgumentNullException(nameof(messageReceiver));

            _queueClientFactory = subscriptionClientFactory;
            _messageReceiver = messageReceiver;
        }

        public async Task<IQueueClient> RegisterClientAsync(string queue)
        {
            await ConfigureQueueAsync(queue);

            _client.RegisterMessageHandler((message, cancelationToken) => _messageReceiver.ReceiveAsync(_client, message, cancelationToken),
                new MessageHandlerOptions(_messageReceiver.OnErrorAsync) { AutoComplete = false });

            return _client;
        }

        public async Task ConfigureQueueAsync(string queue)
        {
            if (_client is not null)
                await _client.CloseAsync();

            _client = _queueClientFactory.Create(queue);
        }
    }
}
