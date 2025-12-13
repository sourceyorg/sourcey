using Shouldly;
using Sourcey.Events;
using Sourcey.Events.Streams;
using Sourcey.Keys;

namespace Sourcey.Tests.Events.Streams;

public sealed record DummyEvent(StreamId StreamId, int? Version, string Name) : Event(StreamId, Version);

public class When_using_event_stream
{
    private static (DummyEvent e1, DummyEvent e2, EventStream stream) Build()
    {
        var s = StreamId.From("s-1");
        var e1 = new DummyEvent(s, 1, "first");
        var e2 = new DummyEvent(s, 2, "second");
        var stream = new EventStream(s, e1, e2);
        return (e1, e2, stream);
    }

    [Then]
    public void Count_and_enumeration_should_match()
    {
        var (_, _, stream) = Build();
        stream.Count.ShouldBe(2);
        stream.ToList().Count.ShouldBe(2);
        ((System.Collections.IEnumerable)stream).Cast<IEvent>().Count().ShouldBe(2);
    }

    [Then]
    public void First_and_last_helpers_should_return_expected_events()
    {
        var (e1, e2, stream) = Build();

        stream.GetFirstOrDefault<DummyEvent>().ShouldBe(e1);
        stream.GetLastOrDefault<DummyEvent>().ShouldBe(e2);

        stream.TryGetFirst<DummyEvent>(out var first).ShouldBeTrue();
        first.ShouldBe(e1);

        stream.TryGetLast<DummyEvent>(out var last).ShouldBeTrue();
        last.ShouldBe(e2);
    }
}
