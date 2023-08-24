using Sourcey.Aggregates;
using Sourcey.Aggregates.Builder;

namespace Sourcey.Redis.Aggregates.Snapshots
{
    public sealed class AggregateSnapshotSettings<TState>
        where TState : IAggregateState
    {
        public string ConnectionString { get; set; }
        public SnapshotExecution SnapshotExecution { get; set; } = SnapshotExecution.Sync;
    }
}
