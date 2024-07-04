using Sourcey.Aggregates;
using Sourcey.Events;
using Sourcey.Keys;
using Xunit.Abstractions;

namespace Sourcey.Tests.Aggregates.Aggregate.Apply;

public class TestAggregateState : IAggregateState
{
    public string Name { get; set; } = string.Empty;
    
    public TestAggregateState() {}

    public TestAggregateState(TestAggregateState state)
    {
        Name = state.Name;
    }
}

public record TestAggregateCreated(StreamId StreamId, int? Version, string Name) : Event(StreamId, Version);

public class TestAggregate : Aggregate<TestAggregateState>
{
    public TestAggregate()
    {
        Handles<TestAggregateCreated>(@event => {
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

public class WhenEventIsHandled : AggregateSpecification<TestAggregate, TestAggregateState>
{
    private readonly string _id = StreamId.New().ToString();
    private readonly string _name = "test";
    public WhenEventIsHandled(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override Task SetupAsync(TestAggregate aggregate)
    {
        aggregate.Create(_id, _name);
        return Task.CompletedTask;
    }

    [Then]
    public void TheAggregateId_Should_BeSet()
    {
        Result.ShouldNotBeNull().Id.ShouldBe(StreamId.From(_id));
    }

    [Then]
    public void TheAggregateVersion_Should_BeNull()
    {
        Result.ShouldNotBeNull().Version.ShouldBeNull();
    }

    [Then]
    public void Then_AggreagteStateName_Should_BeSet()
    {
        Result.ShouldNotBeNull().GetState().Name.ShouldBe(_name);
    }

    [Then]
    public void Then_ThenUncommitedEvents_Should_ContainTheExpectedEvent()
    {
        var @events = Result.ShouldNotBeNull().GetUncommittedEvents();
        @events.ShouldNotBeEmpty();
        @events.Count().ShouldBe(1);

        var @event = @events.FirstOrDefault().ShouldBeOfType<TestAggregateCreated>();
        @event.StreamId.ShouldBe(StreamId.From(_id));
        @event.Version.ShouldBe(1);
        @event.Name.ShouldBe(_name);
    }
}
