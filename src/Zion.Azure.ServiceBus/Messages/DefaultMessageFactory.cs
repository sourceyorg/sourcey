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
        private readonly IEventNotificationSerializer _eventNotificationSerializer;

        public DefaultMessageFactory(IEventNotificationSerializer eventNotificationSerializer)
        {
            if (eventNotificationSerializer == null)
                throw new ArgumentNullException(nameof(eventNotificationSerializer));

            _eventNotificationSerializer = eventNotificationSerializer;
        }

        public Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var eventName = @event.GetType().FriendlyName();

            return CreateMessage(eventName, (IEventNotification<IEvent>)context);
        }
        public Message CreateMessage(IEventNotification<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var eventName = @event.GetType().FriendlyName();

            return CreateMessage(eventName, context);
        }
        private Message CreateMessage(string eventName, IEventNotification<IEvent> context)
        {
            var @event = context.Payload;
            var body = _eventNotificationSerializer.Serialize(context);
            var message = new Message
            {
                MessageId = @event.Id.ToString(),
                Body = body,
                Label = eventName,
                CorrelationId = context.Correlation?.ToString(),
                UserProperties =
                {
                    { nameof(context.StreamId), context.StreamId.ToString() },
                    { nameof(context.Causation), context.Causation?.ToString() },
                    { nameof(context.Actor), context.Actor.ToString() },
                    { nameof(context.Timestamp), context.Timestamp },
                },
            };

            return message;
        }
    }
}
