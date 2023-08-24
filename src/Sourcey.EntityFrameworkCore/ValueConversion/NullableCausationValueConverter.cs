using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ValueConversion
{
    internal sealed class NullableCausationValueConverter : ValueConverter<Causation?, string>
    {
        public NullableCausationValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(Causation? causation)
            => causation.HasValue ? (string)causation.Value : string.Empty;

        private static Causation? ConvertFrom(string causation)
            => string.IsNullOrWhiteSpace(causation) ? null : Causation.From(causation);
    }
}
