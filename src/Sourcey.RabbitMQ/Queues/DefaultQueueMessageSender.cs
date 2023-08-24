using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sourcey.Events;
using Sourcey.Events.Bus;
using Sourcey.RabbitMQ.Connections;
using Sourcey.RabbitMQ.Messages;

namespace Sourcey.RabbitMQ.Queues
{
    internal sealed class DefaultQueueMessageSender : IQueueMessageSender
    {
        private readonly ILogger<DefaultQueueMessageSender> _logger;
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly IMessageFactory _messageFactory;
        private readonly IRabbitMqConnectionFactory _connectionFactory;

        public DefaultQueueMessageSender(ILogger<DefaultQueueMessageSender> logger,
                                         IOptions<RabbitMqOptions> options,
                                         IMessageFactory messageFactory,
                                         IRabbitMqConnectionFactory connectionFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (messageFactory == null)
                throw new ArgumentNullException(nameof(messageFactory));
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _logger = logger;
            _options = options;
            _messageFactory = messageFactory;
            _connectionFactory = connectionFactory;
        }

        public async Task SendAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var message = _messageFactory.CreateMessage(context);

            using (var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken))
            {
                await connection.PublishAsync(message, cancellationToken);
            }
        }
        
        public async Task SendAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));

            var messages = contexts.Select(context => _messageFactory.CreateMessage(context));

            using (var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken))
            {
                await connection.PublishAsync(messages, cancellationToken);
            }
        }
    }
}
