using InMemory.Events;
using Sourcey.Aggregates;
using Sourcey.Keys;

namespace InMemory.Aggregates;

public class SampleAggreagte: Aggregate<SampleState>
{
    public SampleAggreagte(SampleState state): base(state)
    {
        Handles<SomethingHappened>(@event =>
        {
            Id = @event.StreamId;
            _state.Something = @event.Something;
        });
    }

    public override SampleState GetState() => new(_state);

    public void MakeSomethingHappen(string something)
        => Apply(new SomethingHappened(
            StreamId: StreamId.New(),
            Version: Version.GetValueOrDefault() + 1,
            Something: something 
        ));
} 