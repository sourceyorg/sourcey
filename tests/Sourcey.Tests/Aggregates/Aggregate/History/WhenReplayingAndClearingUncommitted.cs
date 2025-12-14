using Sourcey.Aggregates;
using Sourcey.Events;
using Sourcey.Keys;
using Xunit.Abstractions;

namespace Sourcey.Tests.Aggregates.Aggregate.History;

public class ReplayState : IAggregateState
{
    public int Applied { get; set; }
}

public record ReplayEvent(StreamId StreamId, int? Version) : Event(StreamId, Version);

public class ReplayAggregate : Aggregate<ReplayState>
{
    public ReplayAggregate()
    {
        Handles<ReplayEvent>(_ => _state.Applied++);
    }

    public override ReplayState GetState() => _state;
}

public class WhenReplayingAndClearingUncommitted
{
    [Then]
    public void Apply_isNew_false_sets_version_and_does_not_enqueue()
    {
        var sut = new ReplayAggregate();
        sut.Apply(new ReplayEvent(StreamId.From("s"), 5), isNew: false);

        sut.Version.ShouldBe(5);
        sut.GetUncommittedEvents().Count().ShouldBe(0);
        sut.GetState().Applied.ShouldBe(1); // handler executed
    }

    [Then]
    public void FromHistory_replays_and_sets_final_version()
    {
        var sut = new ReplayAggregate();
        sut.FromHistory(new IEvent[]
        {
            new ReplayEvent(StreamId.From("s"), 1),
            new ReplayEvent(StreamId.From("s"), 2),
            new ReplayEvent(StreamId.From("s"), 3)
        });

        sut.Version.ShouldBe(3);
        sut.GetUncommittedEvents().Count().ShouldBe(0);
        sut.GetState().Applied.ShouldBe(3);
    }

    [Then]
    public void ClearUncommittedEvents_empties_queue()
    {
        var sut = new ReplayAggregate();

        // isNew true enqueues
        sut.Apply(new ReplayEvent(StreamId.From("s"), 1));
        sut.Apply(new ReplayEvent(StreamId.From("s"), 2));
        sut.GetUncommittedEvents().Count().ShouldBe(2);

        sut.ClearUncommittedEvents();
        sut.GetUncommittedEvents().Count().ShouldBe(0);
    }
}
