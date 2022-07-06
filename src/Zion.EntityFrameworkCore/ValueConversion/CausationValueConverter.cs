using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Core.Keys;

namespace Zion.EntityFrameworkCore.ValueConversion
{
    internal sealed class CausationValueConverter : ValueConverter<Causation, string>
    {
        public CausationValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(Causation casuation)
            => (string)casuation;

        private static Causation ConvertFrom(string causation)
            => Causation.From(causation);
    }
}
