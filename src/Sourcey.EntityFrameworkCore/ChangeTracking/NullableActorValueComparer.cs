using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ChangeTracking;

internal sealed class NullableActorValueComparer : ValueComparer<Actor?>
{
    public NullableActorValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(Actor? left, Actor? right)
    {
        if (left == null || right == null)
            return false;

        return left.Value.Equals(right.Value);
    }

    private static int HashCode(Actor? actor)
    {
        if (actor == null)
            return 0;

        if (actor is IEquatable<Actor>)
            return actor.GetHashCode();

        return ConvertTo(actor)?.GetHashCode() ?? 0;
    }

    private static Actor? CreateSnapshot(Actor? actor)
    {
        return ConvertFrom(ConvertTo(actor));
    }

    private static string? ConvertTo(Actor? actor)
    {
        if (actor == null)
            return null;

        return (string)actor;
    }

    private static Actor? ConvertFrom(string? actor)
    {
        if (string.IsNullOrWhiteSpace(actor))
            return null;

        return Actor.From(actor);
    }
}
