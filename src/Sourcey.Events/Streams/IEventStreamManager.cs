﻿using Sourcey.Events.Stores;

namespace Sourcey.Events.Streams;

public interface IEventStreamManager
{
    void Append(params IEventContext<IEvent>[] events);
    bool TryGet(StreamId streamId, out EventStream? eventStream);
    EventStream? GetMostRecentOrDefault();
    StreamId? GetMostRecentStreamId();
}
