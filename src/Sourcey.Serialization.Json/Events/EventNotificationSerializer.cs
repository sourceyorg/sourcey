﻿using System.Text.Json;
using Sourcey.Events;
using Sourcey.Events.Bus;
using Sourcey.Events.Serialization;

namespace Sourcey.Serialization.Json.Events
{
    internal class EventNotificationSerializer : IEventNotificationSerializer
    {
        private readonly IEventSerializer _eventSerializer;

        public EventNotificationSerializer(IEventSerializer eventSerializer)
        {
            _eventSerializer = eventSerializer;
        }

        public byte[] Serialize<T>(IEventNotification<T> data) where T : IEvent
        {
            var body = new EventNotificationPayload
            {
                Actor = data.Actor,
                Causation = data.Causation,
                Correlation = data.Correlation,
                Payload = _eventSerializer.Serialize(data.Payload),
                StreamId = data.StreamId,
                Timestamp = data.Timestamp
            };

            return JsonSerializer.SerializeToUtf8Bytes(body);
        }
    }
}