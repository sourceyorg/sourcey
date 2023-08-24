using Sourcey.Aggregates;
using Sourcey.Events.Streams;

namespace Sourcey.Redis.Aggregates.Tests.Snapshots
{
    public sealed class SnapshotAggregate : Aggregate<SnapshotAggregateState>
    {
        public SnapshotAggregate(SnapshotAggregateState state) : base(state)
        {
            Id = StreamId.New();
        }

        public override SnapshotAggregateState GetState() => new();
    }

    public sealed class SnapshotAggregateState : IAggregateState
    {
    }
}
