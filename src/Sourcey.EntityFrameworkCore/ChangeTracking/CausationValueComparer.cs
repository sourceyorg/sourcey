using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ChangeTracking;

internal sealed class CausationValueComparer : ValueComparer<Causation>
{
    public CausationValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(Causation left, Causation right)
        => left.Equals(right);
    private static int HashCode(Causation causation)
        => causation.GetHashCode();
    private static Causation CreateSnapshot(Causation causation)
        => ConvertFrom(ConvertTo(causation));
    private static string ConvertTo(Causation causation)
        => (string)causation;
    private static Causation ConvertFrom(string causation)
        => Causation.From(causation);
}
