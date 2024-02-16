using Sourcey.Aggregates;

namespace Sourcey.Testing.Integration.Stubs.Aggregates;

public class SampleState: IAggregateState
{
    public string? Something { get; set; }
    public SampleState() {}

    public SampleState(SampleState state)
    {
        Something = state.Something;
    }
}
