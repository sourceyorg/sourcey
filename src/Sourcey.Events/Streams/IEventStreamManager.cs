using Sourcey.Events.Stores;

namespace Sourcey.Events.Streams;

public interface IEventStreamManager
{
    void Append(params IEventContext<IEvent>[] events);
    bool TryGet(StreamId streamId, out EventStream? eventStream);
    EventStream? GetMostRecentOrDefault();
    StreamId? GetMostRecentStreamId();
    bool TryGetLastEvent<TEvent>(StreamId streamId, out TEvent? @event);
    bool TryGetFirstEvent<TEvent>(StreamId streamId, out TEvent? @event);
    TEvent? GetFirstOrDefault<TEvent>(StreamId streamId);
    TEvent? GetLastOrDefault<TEvent>(StreamId streamId);
}
