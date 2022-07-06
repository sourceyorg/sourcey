using Zion.Events.Stores;

namespace Zion.Events.Streams
{
    public interface IEventStreamManager
    {
        void Append(params IEventContext<IEvent>[] events);
        EventStream? GetMostRecentOrDefault();
        StreamId? GetMostRecentStreamId();
    }
}
