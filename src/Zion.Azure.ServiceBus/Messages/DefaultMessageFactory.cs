using System.Text;
using Microsoft.Azure.ServiceBus;
using Zion.Core.Extensions;
using Zion.Events;
using Zion.Events.Bus;
using Zion.Events.Serialization;

namespace Zion.Azure.ServiceBus.Messages
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
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var eventName = @event.GetType().FriendlyFullName();

            return CreateMessage(eventName, (IEventNotification<IEvent>)context);
        }
        public Message CreateMessage(IEventNotification<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var eventName = @event.GetType().FriendlyFullName();

            return CreateMessage(eventName, context);
        }
        private Message CreateMessage(string eventName, IEventNotification<IEvent> context)
        {
            var @event = context.Payload;
            var body = _eventSerializer.Serialize(@event);
            var message = new Message
            {
                MessageId = @event.Id.ToString(),
                Body = Encoding.UTF8.GetBytes(body),
                Label = eventName,
                CorrelationId = context.Correlation?.ToString(),
                UserProperties =
                {
                    { nameof(context.StreamId), context.StreamId.ToString() },
                    { nameof(context.Causation), context.Causation?.ToString() },
                    { nameof(context.Actor), context.Actor },
                    { nameof(context.Timestamp), context.Timestamp },
                },
            };

            return message;
        }
    }
}
