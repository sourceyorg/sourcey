namespace Sourcey.Aggregates.Builder;

/// <summary>
/// Represents the execution mode for taking snapshots of an aggregate.
/// </summary>
public enum SnapshotExecution
{
    /// <summary>
    /// Synchronously takes a snapshot of the aggregate.
    /// </summary>
    Sync,    

    /// <summary>
    /// Buffers the snapshot and takes it at a later time.
    /// </summary>
    Buffered
}
