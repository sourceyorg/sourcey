using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Core.Keys;

namespace Zion.EntityFrameworkCore.ValueConversion
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
