﻿using Zion.Events.Stores;

namespace Zion.Events.Streams
{
    internal sealed class EventStreamManager : IEventStreamManager
    {
        private readonly List<StreamId> _streamIds = new();
        private readonly Dictionary<StreamId, List<IEvent>> _events = new();

        public void Append(params IEventContext<IEvent>[] events)
        {
            foreach (var stream in events.GroupBy(ec => ec.StreamId))
            {
                var streamId = StreamId.From(stream.Key);

                if (!_events.TryGetValue(streamId, out var storedEvents))
                    _streamIds.Add(streamId);
                else
                    storedEvents = new();

                storedEvents.AddRange(stream.Select(e => e.Payload));

                _events[streamId] = storedEvents;
            }
        }

        public EventStream? GetMostRecentOrDefault()
        {
            var streamId = GetMostRecentStreamId();

            if (!streamId.HasValue)
                return null;

            return new EventStream(streamId.Value, _events[streamId.Value].ToArray());
        }

        public StreamId? GetMostRecentStreamId()
        {
            if (_streamIds.Count < 1)
                return null;

            return _streamIds.LastOrDefault();
        }
    }
}
