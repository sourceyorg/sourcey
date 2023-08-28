using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.ChangeTracking;

internal sealed class NullableCausationValueComparer : ValueComparer<Causation?>
{
    public NullableCausationValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(Causation? left, Causation? right)
    {
        if (left == null || right == null)
            return false;

        return left.Value.Equals(right.Value);
    }

    private static int HashCode(Causation? causation)
    {
        if (causation == null)
            return 0;

        if (causation is IEquatable<Causation>)
            return causation.GetHashCode();

        return ConvertTo(causation)?.GetHashCode() ?? 0;
    }

    private static Causation? CreateSnapshot(Causation? causation)
    {
        return ConvertFrom(ConvertTo(causation));
    }

    private static string? ConvertTo(Causation? causation)
    {
        if (causation == null)
            return null;

        return (string)causation;
    }

    private static Causation? ConvertFrom(string? causation)
    {
        if (string.IsNullOrWhiteSpace(causation))
            return null;

        return Causation.From(causation);
    }
}
