namespace Sourcey.Aggregates.Builder;


/// <summary>
/// Represents the type of permutation to be used when configuring auto conflict resolution for agregates.
/// </summary>
public enum PermutationType
{
    Multiple = 0,
    Single = 2,
    Duplicates = 4
}
