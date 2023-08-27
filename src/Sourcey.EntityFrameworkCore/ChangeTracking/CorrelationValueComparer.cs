using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ChangeTracking;

internal sealed class CorrelationValueComparer : ValueComparer<Correlation>
{
    public CorrelationValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(Correlation left, Correlation right)
        => left.Equals(right);

    private static int HashCode(Correlation correlation)
        => correlation.GetHashCode();
    private static Correlation CreateSnapshot(Correlation correlation)
        => ConvertFrom(ConvertTo(correlation));
    private static string ConvertTo(Correlation correlation)
        => (string)correlation;
    private static Correlation ConvertFrom(string correlation)
        => Correlation.From(correlation);
}
