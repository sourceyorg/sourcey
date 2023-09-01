using Sourcey.Aggregates;
using Sourcey.Events;
using Sourcey.Keys;
using Xunit.Abstractions;

namespace Sourcey.Tests.Aggregates.Aggregate.History;

public class TestAggregateState : IAggregateState
{
    public string Name { get; set; } = string.Empty;

    public TestAggregateState() { }

    public TestAggregateState(TestAggregateState state)
    {
        Name = state.Name;
    }
}

public record TestAggregateCreated(StreamId StreamId, int? Version, string Name) : Event(StreamId, Version);

public class TestAggregate : Aggregate<TestAggregateState>
{
    public TestAggregate(TestAggregateState state) : base(state)
    {
        Handles<TestAggregateCreated>(@event =>
        {
            Id = @event.StreamId;
            _state.Name = @event.Name;
        });
    }

    public override TestAggregateState GetState() => new(_state);

    public void Create(string id, string name)
        => Apply(new TestAggregateCreated(
                StreamId.From(id),
                Version.GetValueOrDefault() + 1,
                name
            ));
}

public class WhenAggregateHasHistory : AggregateSpecification<TestAggregate, TestAggregateState>
{
    private readonly string _id = StreamId.New().ToString();
    private readonly string _name = "test";

    public WhenAggregateHasHistory(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override Task SetupAsync(TestAggregate aggregate)
    {
        aggregate.FromHistory(new IEvent[1] {
            new TestAggregateCreated(
                StreamId.From(_id),
                1,
                "test"
            )
        });

        return Task.CompletedTask;
    }

    [Then]
    public void TheAggregateId_Should_BeSet()
    {
        Result.ShouldNotBeNull().Id.ShouldBe(StreamId.From(_id));
    }

    [Then]
    public void TheAggregateVersion_Should_BeSet()
    {
        Result.ShouldNotBeNull().Version.ShouldBe(1);
    }

    [Then]
    public void Then_AggreagteStateName_Should_BeSet()
    {
        Result.ShouldNotBeNull().GetState().Name.ShouldBe(_name);
    }

    [Then]
    public void Then_ThenUncommitedEvents_Should_NotContainAnyUncommitedEvents()
    {
        var @events = Result.ShouldNotBeNull().GetUncommittedEvents();
        @events.ShouldBeEmpty();
    }
}