namespace Sourcey.Aggregates.Concurrency;

/// <summary>
/// Specifies the action to take when a concurrency conflict occurs.
/// </summary>
public enum ConflictAction
{
    /// <summary>
    /// Throws a concurrency exception when a conflict occurs.
    /// </summary>
    Throw,

    /// <summary>
    /// Ignores the conflict and continues processing.
    /// </summary>
    Pass
}