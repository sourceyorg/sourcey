using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Events;
using Sourcey.Events.Streams;

namespace Sourcey.EntityFrameworkCore.Events.ValueConversion;

internal sealed class StreamIdValueConverter : ValueConverter<StreamId, string>
{
    public StreamIdValueConverter(ConverterMappingHints mappingHints = default)
        : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
    {
    }

    private static string ConvertTo(StreamId streamId)
        => (string)streamId;

    private static StreamId ConvertFrom(string streamId)
        => StreamId.From(streamId);
}
