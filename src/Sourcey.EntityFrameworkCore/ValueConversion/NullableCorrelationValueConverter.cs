using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ValueConversion
{
    internal sealed class NullableCorrelationValueConverter : ValueConverter<Correlation?, string>
    {
        public NullableCorrelationValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(Correlation? correlation)
            => correlation.HasValue ? (string)correlation.Value : string.Empty;

        private static Correlation? ConvertFrom(string correlation)
            => string.IsNullOrWhiteSpace(correlation) ? null : Correlation.From(correlation);
    }
}
