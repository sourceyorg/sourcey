using Sourcey.Events.Stores;

namespace Sourcey.Events.Streams;

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
            {
                storedEvents = new();
                _streamIds.Add(streamId);
            }

            storedEvents.AddRange(stream.Select(e => e.Payload));

            _events[streamId] = storedEvents;
        }
    }

    public EventStream? GetMostRecentOrDefault()
    {
        var streamId = GetMostRecentStreamId();

        if (!streamId.HasValue)
            return null;

        if (!TryGet(streamId.Value, out var stream))
            return null;

        return stream;
    }

    public bool TryGet(StreamId streamId, out EventStream? eventStream)
    {
        eventStream = null;

        if (!_events.TryGetValue(streamId, out var events))
            return false;

        eventStream = new EventStream(streamId, events.ToArray());

        return true;
    }

    public StreamId? GetMostRecentStreamId()
    {
        if (_streamIds.Count < 1)
            return null;

        return _streamIds.LastOrDefault();
    }

    public bool TryGetLastEvent<TEvent>(StreamId streamId, out TEvent? @event)
    {
        @event = default;

        if (!TryGet(streamId, out var stream) || stream is null)
            return false;
        
        if(!stream.TryGetLast(out @event))
            return false;
        
        return @event is not null;
    }

    public bool TryGetFirstEvent<TEvent>(StreamId streamId, out TEvent? @event)
    {
        @event = default;

        if (!TryGet(streamId, out var stream) || stream is null)
            return false;
        
        if(!stream.TryGetFirst(out @event))
            return false;
        
        return @event is not null;
    }

    public TEvent? GetFirstOrDefault<TEvent>(StreamId streamId)
    {
        if (!TryGet(streamId, out var stream) || stream is null)
            return default;

        var @event = stream.GetFirstOrDefault<TEvent>();

        return @event is null ? default : @event;
    }

    public TEvent? GetLastOrDefault<TEvent>(StreamId streamId)
    {
        if (!TryGet(streamId, out var stream) || stream is null)
            return default;

        var @event = stream.GetLastOrDefault<TEvent>();

        return @event is null ? default : @event;
    }
}
