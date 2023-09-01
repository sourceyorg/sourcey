using Sourcey.Events;
using Sourcey.Keys;

namespace EntityFrameworkCore.Events;

public record SomethingHappened(StreamId StreamId, int? Version, string Something)
    : Event(StreamId, Version);