using Sourcey.Events;
using Sourcey.Keys;

namespace InMemory.Events;

public record SomethingHappened(StreamId StreamId, int? Version, string Something)
    : Event(StreamId, Version);