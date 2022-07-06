using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Events;

namespace Zion.EntityFrameworkCore.Events.ValueConversion
{
    internal sealed class NullableEventIdValueConverter : ValueConverter<EventId?, string>
    {
        public NullableEventIdValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(EventId? eventId)
            => eventId.HasValue ? (string)eventId.Value : string.Empty;

        private static EventId? ConvertFrom(string eventId)
            => string.IsNullOrWhiteSpace(eventId) ? null : EventId.From(eventId);
    }
}
