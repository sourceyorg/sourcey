using Sourcey.Keys;

namespace Sourcey.Events.Stores;

public sealed class Page
{
    public long Offset { get; }
    public long PreviousOffset { get; }
    public IEnumerable<KeyValuePair<StreamId, IEnumerable<IEventContext<IEvent>>>> Events { get; }

    public Page(long offset, long previousOffset, IEnumerable<KeyValuePair<StreamId, IEnumerable<IEventContext<IEvent>>>> events)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        Offset = offset;
        PreviousOffset = previousOffset;
        Events = events;
    }
}
