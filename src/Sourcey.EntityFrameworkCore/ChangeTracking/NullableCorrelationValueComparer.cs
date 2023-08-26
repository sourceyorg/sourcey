using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ChangeTracking;

internal sealed class NullableCorrelationValueComparer : ValueComparer<Correlation?>
{
    public NullableCorrelationValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(Correlation? left, Correlation? right)
    {
        if (left == null || right == null)
            return false;

        return left.Value.Equals(right.Value);
    }

    private static int HashCode(Correlation? correlation)
    {
        if (correlation == null)
            return 0;

        if (correlation is IEquatable<Correlation>)
            return correlation.GetHashCode();

        return ConvertTo(correlation)?.GetHashCode() ?? 0;
    }

    private static Correlation? CreateSnapshot(Correlation? correlation)
    {
        return ConvertFrom(ConvertTo(correlation));
    }

    private static string? ConvertTo(Correlation? correlation)
    {
        if (correlation == null)
            return null;

        return (string)correlation;
    }

    private static Correlation? ConvertFrom(string? correlation)
    {
        if (string.IsNullOrWhiteSpace(correlation))
            return null;

        return Correlation.From(correlation);
    }
}
