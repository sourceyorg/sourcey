using System.Text;
using Zion.Core.Extensions;
using Zion.Events;
using Zion.Events.Bus;
using Zion.Events.Serialization;

namespace Zion.RabbitMQ.Messages
{
    internal sealed class DefaultMessageFactory : IMessageFactory
    {
        private readonly IEventSerializer _eventSerializer;

        public DefaultMessageFactory(IEventSerializer eventSerializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));
          
            _eventSerializer = eventSerializer;
        }

        public Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var eventName = typeof(TEvent).FriendlyFullName();

            return CreateMessage(eventName, (IEventNotification<IEvent>)context);
        }
        public Message CreateMessage(IEventNotification<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var eventName = context.Payload.GetType().FriendlyFullName();

            return CreateMessage(eventName, context);
        }

        private Message CreateMessage(string eventName, IEventNotification<IEvent> context)
        {
            var @event = context.Payload;
            var body = _eventSerializer.Serialize(context.Payload);

            return new Message
            {
                MessageId = @event.Id,
                Type = eventName,
                CorrelationId = context.Correlation,
                CausationId = context.Causation,
                UserId = context.Actor,
                Body = Encoding.UTF8.GetBytes(body),
            };
        }
    }
}
