using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Events;

namespace Sourcey.EntityFrameworkCore.Events.ValueConversion
{
    internal sealed class EventIdValueConverter : ValueConverter<EventId, string>
    {
        public EventIdValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(EventId eventId)
            => (string)eventId;

        private static EventId ConvertFrom(string eventId)
            => EventId.From(eventId);
    }
}
