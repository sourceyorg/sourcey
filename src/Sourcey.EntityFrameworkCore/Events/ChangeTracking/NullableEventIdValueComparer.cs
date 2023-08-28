using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.Events.ChangeTracking;

internal sealed class NullableEventIdValueComparer : ValueComparer<EventId?>
{
    public NullableEventIdValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(EventId? left, EventId? right)
    {
        if (left == null || right == null)
            return false;

        return left.Value.Equals(right.Value);
    }

    private static int HashCode(EventId? eventId)
    {
        if (eventId == null)
            return 0;

        if (eventId is IEquatable<EventId>)
            return eventId.GetHashCode();

        return ConvertTo(eventId)?.GetHashCode() ?? 0;
    }

    private static EventId? CreateSnapshot(EventId? eventId)
    {
        return ConvertFrom(ConvertTo(eventId));
    }

    private static string? ConvertTo(EventId? eventId)
    {
        if (eventId == null)
            return null;

        return (string)eventId;
    }

    private static EventId? ConvertFrom(string? eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            return null;

        return EventId.From(eventId);
    }
}
