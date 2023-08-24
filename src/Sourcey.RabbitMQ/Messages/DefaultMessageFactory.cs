using System.Text;
using Sourcey.Core.Extensions;
using Sourcey.Events;
using Sourcey.Events.Bus;
using Sourcey.Events.Serialization;

namespace Sourcey.RabbitMQ.Messages
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
            var body = _eventNotificationSerializer.Serialize(context);

            return new Message
            {
                MessageId = @event.Id,
                Type = eventName,
                CorrelationId = context.Correlation,
                CausationId = context.Causation,
                Actor = context.Actor,
                Body = body,
                StreamId = context.StreamId
            };
        }
    }
}
