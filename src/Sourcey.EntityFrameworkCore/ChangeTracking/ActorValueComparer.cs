using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.ChangeTracking;

internal sealed class ActorValueComparer : ValueComparer<Actor>
{
    public ActorValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(Actor left, Actor right)
        => left.Equals(right);
    private static int HashCode(Actor actor)
        => actor.GetHashCode();
    private static Actor CreateSnapshot(Actor actor)
        => ConvertFrom(ConvertTo(actor));
    private static string ConvertTo(Actor actor)
        => (string)actor;
    private static Actor ConvertFrom(string actor)
        => Actor.From(actor);
}
