using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Core.Keys;
using Sourcey.Events;

namespace Sourcey.EntityFrameworkCore.Events.ChangeTracking
{
    internal sealed class EventIdValueComparer : ValueComparer<EventId>
    {
        public EventIdValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(EventId left, EventId right)
            => left.Equals(right);
        private static int HashCode(EventId eventId)
            => eventId.GetHashCode();
        private static EventId CreateSnapshot(EventId eventId)
            => ConvertFrom(ConvertTo(eventId));
        private static string ConvertTo(EventId eventId)
            => (string)eventId;
        private static EventId ConvertFrom(string eventId)
            => EventId.From(eventId);
    }
}
