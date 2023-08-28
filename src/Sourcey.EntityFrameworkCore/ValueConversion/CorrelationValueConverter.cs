using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.ValueConversion;

internal sealed class CorrelationValueConverter : ValueConverter<Correlation, string>
{
    public CorrelationValueConverter(ConverterMappingHints mappingHints = default)
        : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
    {
    }

    private static string ConvertTo(Correlation correlation)
        => (string)correlation;

    private static Correlation ConvertFrom(string correlation)
        => Correlation.From(correlation);
}
