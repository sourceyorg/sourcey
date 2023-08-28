using System.Collections;
using Sourcey.Keys;

namespace Sourcey.Events.Streams;

public record EventStream : IReadOnlyCollection<IEvent>
{
    private readonly IReadOnlyCollection<IEvent> _events;

    public StreamId StreamId { get; }

    public int Count => _events.Count;

    public EventStream(StreamId streamId, params IEvent[] events)
    {
        StreamId = streamId;
        _events = events;
    }

    public IEnumerator<IEvent> GetEnumerator()
        => _events.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public bool TryGetLast<TEvent>(out TEvent? @event)
    {
        @event = GetLastOrDefault<TEvent>();
        return @event is not null;
    }

    public bool TryGetFirst<TEvent>(out TEvent? @event)
    {
        @event = GetFirstOrDefault<TEvent>();
        return @event is not null;
    }

    public TEvent? GetFirstOrDefault<TEvent>()
    {
        var type = typeof(TEvent);
        return (TEvent?)_events?.FirstOrDefault(e => e.GetType() == type);
    }

    public TEvent? GetLastOrDefault<TEvent>()
    {
        var type = typeof(TEvent);
        return (TEvent?)_events?.LastOrDefault(e => e.GetType() == type);
    }
}
