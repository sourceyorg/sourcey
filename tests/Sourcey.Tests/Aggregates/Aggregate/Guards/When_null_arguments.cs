using Sourcey.Aggregates;
using Sourcey.Events;
using Sourcey.Keys;
using Xunit.Abstractions;

namespace Sourcey.Tests.Aggregates.Aggregate.Guards;

public class GuardAggregateState : IAggregateState { }

public record GuardEvent(StreamId StreamId, int? Version) : Event(StreamId, Version);

public class GuardAggregate : Aggregate<GuardAggregateState>
{
    public override GuardAggregateState GetState() => new();
}

public class When_null_arguments
{
    [Then]
    public void Apply_null_should_throw()
    {
        var sut = new GuardAggregate();
        Should.Throw<ArgumentNullException>(() => sut.Apply(null!));
        Should.Throw<ArgumentNullException>(() => sut.Apply(null!, isNew: true));
    }

    [Then]
    public void FromHistory_null_should_throw()
    {
        var sut = new GuardAggregate();
        Should.Throw<ArgumentNullException>(() => sut.FromHistory(null!));
    }
}
