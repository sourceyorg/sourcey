using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Encryption;

namespace Sourcey.EntityFrameworkCore.Encryption.ValueConversion
{
    internal sealed class NullableSecretValueConverter : ValueConverter<Secret?, byte[]?>
    {
        public NullableSecretValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static byte[]? ConvertTo(Secret? secret)
            => secret.HasValue ? (byte[]?)secret.Value : null;

        private static Secret? ConvertFrom(byte[]? secret)
            => secret is null ? null : Secret.From(secret);
    }
}
