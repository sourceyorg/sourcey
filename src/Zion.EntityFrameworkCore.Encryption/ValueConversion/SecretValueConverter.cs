using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Encryption;

namespace Zion.EntityFrameworkCore.Encryption.ValueConversion
{
    internal sealed class SecretValueConverter : ValueConverter<Secret, byte[]>
    {
        public SecretValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static byte[] ConvertTo(Secret secret)
            => (byte[])secret;

        private static Secret ConvertFrom(byte[] secret)
            => Secret.From(secret);
    }
}
