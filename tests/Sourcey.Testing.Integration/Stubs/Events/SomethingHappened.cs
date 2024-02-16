using Sourcey.Events;
using Sourcey.Keys;

namespace Sourcey.Testing.Integration.Stubs.Events;

public record SomethingHappened(StreamId StreamId, int? Version, string Something)
    : Event(StreamId, Version);
