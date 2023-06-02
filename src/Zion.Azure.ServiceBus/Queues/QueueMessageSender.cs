using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Zion.Azure.ServiceBus.Messages;
using Zion.Events;
using Zion.Events.Bus;

namespace Zion.Azure.ServiceBus.Queues
{
    internal sealed class QueueMessageSender : IQueueMessageSender
    {
        private const int MAX_SERVICE_BUS_MESSAGE_SIZE = 192000;

        private readonly IQueueClientFactory _queueClientFactory;
        private readonly IMessageFactory _messageFactory;
        private readonly ILogger<QueueMessageSender> _logger;

        public QueueMessageSender(ILogger<QueueMessageSender> logger,
                                         IQueueClientFactory topicClientFactory,
                                         IMessageFactory messageFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (topicClientFactory == null)
                throw new ArgumentNullException(nameof(topicClientFactory));
            if (messageFactory == null)
                throw new ArgumentNullException(nameof(messageFactory));
            
            _logger = logger;
            _queueClientFactory = topicClientFactory;
            _messageFactory = messageFactory;
        }

        public async Task SendAsync<TEvent>(string queue, IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = _queueClientFactory.Create(queue);
            var message = _messageFactory.CreateMessage(context);

            _logger.LogInformation($"Sending message 1 of 1. Type: '{message.Label}' | Size: '{message.Size}' bytes");

            await client.SendAsync(message);
            await client.CloseAsync();
        }
        public async Task SendAsync(string queue, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));

            cancellationToken.ThrowIfCancellationRequested();

            var messages = contexts.Select(context => _messageFactory.CreateMessage(context));
            var batchedMessages = messages.Aggregate(new { Sum = 0L, Current = (List<Message>)null, Result = new List<List<Message>>() }, (agg, message) =>
            {
                var size = message.Size;

                if (agg.Current == null || agg.Sum + size > MAX_SERVICE_BUS_MESSAGE_SIZE)
                {
                    var current = new List<Message> { message };

                    agg.Result.Add(current);

                    return new { Sum = size, Current = current, agg.Result };
                }

                agg.Current.Add(message);

                return new { Sum = agg.Sum + size, agg.Current, agg.Result };
            }).Result;

            var client = _queueClientFactory.Create(queue);

            _logger.LogInformation($"Sending batched messages 1 of {messages.Count()}.");

            var tasks = batchedMessages.Select(async (batch, index) =>
            {
                _logger.LogInformation($"Sending batch {index + 1} of {batchedMessages.Count}. Message count: {batch.Count}.");

                await client.SendAsync(batch);
            });

            await Task.WhenAll(tasks);

            _logger.LogInformation($"Sent batched messages 1 of {messages.Count()} in {batchedMessages.Count} batches.");
            await client.CloseAsync();
        }
    }
}
