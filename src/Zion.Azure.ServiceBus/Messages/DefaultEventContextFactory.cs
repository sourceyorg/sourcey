using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Zion.Core.Keys;
using Zion.Events;
using Zion.Events.Cache;
using Zion.Events.Serialization;
using Zion.Events.Stores;
using Zion.Events.Streams;

namespace Zion.Azure.ServiceBus.Messages
{
    internal sealed class DefaultEventContextFactory : IEventContextFactory
    {
        private readonly ConcurrentDictionary<Type, Activator<IEventContext<IEvent>>> _cache;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly IEventNotificationDeserializer _eventNotificationDeserializer;

        public DefaultEventContextFactory(IEventTypeCache eventTypeCache,
                                          IEventNotificationDeserializer eventNotificationDeserializer)
        {
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (eventNotificationDeserializer == null)
                throw new ArgumentNullException(nameof(eventNotificationDeserializer));

            _eventTypeCache = eventTypeCache;
            _eventNotificationDeserializer = eventNotificationDeserializer;
            _cache = new ConcurrentDictionary<Type, Activator<IEventContext<IEvent>>>();
        }

        public IEventContext<IEvent> CreateContext(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!_eventTypeCache.TryGet(message.Label, out var type))
                throw new ArgumentException($"Could not find event type for '{message.Label}'");

            var eventNotification = _eventNotificationDeserializer.Deserialize(message.Body, type);

            if (_cache.TryGetValue(type, out var activator))
                return activator(eventNotification.StreamId, eventNotification.Payload, eventNotification.Correlation, eventNotification.Causation, eventNotification.Payload.Timestamp, eventNotification.Actor, null);

            activator = BuildActivator(typeof(EventContext<>).MakeGenericType(type));

            _cache.TryAdd(type, activator);

            return activator(eventNotification.StreamId, eventNotification.Payload, eventNotification.Correlation, eventNotification.Causation, eventNotification.Payload.Timestamp, eventNotification.Actor, null);
        }

        private Activator<IEventContext<IEvent>> BuildActivator(Type type)
        {
            var expectedParameterTypes = new Type[] { typeof(StreamId), type.GenericTypeArguments[0], typeof(Correlation?), typeof(Causation?), typeof(DateTimeOffset), typeof(Actor), typeof(DateTimeOffset?) };
            var constructor = type.GetConstructor(expectedParameterTypes);

            if (constructor == null)
                throw new InvalidOperationException($"Could not find expected constructor on type '{type.FullName}'. Expecting the following parameters: '{string.Join("' ,", expectedParameterTypes.Select(t => t.Name))}'");

            var parameters = constructor.GetParameters();
            var args = Expression.Parameter(typeof(object[]), "args");
            var argsExpressions = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var accessor = Expression.ArrayIndex(args, Expression.Constant(i));
                var convert = Convert(accessor, parameterType);
                argsExpressions[i] = convert;
            }

            var newExpression = Expression.New(constructor, argsExpressions);
            var lambda = Expression.Lambda(typeof(Activator<IEventContext<IEvent>>), newExpression, args);

            return (Activator<IEventContext<IEvent>>)lambda.Compile();
        }

        private static Expression Convert(Expression expression, Type type)
        {
            if (type.GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
                return expression;

            return Expression.Convert(expression, type);
        }

        private delegate T Activator<T>(params object[] args);
    }
}
