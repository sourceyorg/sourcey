using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Events.Streams;

namespace Sourcey.EntityFrameworkCore.Events.ValueConversion;

internal sealed class NullableStreamIdValueConverter : ValueConverter<StreamId?, string>
{
    public NullableStreamIdValueConverter(ConverterMappingHints mappingHints = default)
        : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
    {
    }

    private static string ConvertTo(StreamId? streamId)
        => streamId.HasValue ? (string)streamId.Value : string.Empty;

    private static StreamId? ConvertFrom(string streamId)
        => string.IsNullOrWhiteSpace(streamId) ? null : StreamId.From(streamId);
}
