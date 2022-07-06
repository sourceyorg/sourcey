using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.Encryption;
using Zion.EntityFrameworkCore.Encryption.ChangeTracking;
using Zion.EntityFrameworkCore.Encryption.ValueConversion;

namespace Zion.EntityFrameworkCore.Encryption.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<Secret> HasSecretValueConversion(this PropertyBuilder<Secret> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new SecretValueConverter())
                   .Metadata
                   .SetValueComparer(new SecretValueComparer());

            return builder;
        }

        public static PropertyBuilder<Secret?> HasNullableSecretValueConversion(this PropertyBuilder<Secret?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableSecretValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableSecretValueComparer());

            return builder;
        }
    }
}
