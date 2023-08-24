using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ValueConversion
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
