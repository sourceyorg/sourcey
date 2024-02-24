using Sourcey.Aggregates;
using Sourcey.Keys;
using Sourcey.Testing.Integration.Stubs.Events;

namespace Sourcey.Testing.Integration.Stubs.Aggregates;

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

    public void MakeSomethingHappen(StreamId streamId, string something)
        => Apply(new SomethingHappened(
            StreamId: streamId,
            Version: Version.GetValueOrDefault() + 1,
            Something: something 
        ));
}
