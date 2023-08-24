using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sourcey.Encryption;
using Sourcey.EntityFrameworkCore.Encryption.ChangeTracking;
using Sourcey.EntityFrameworkCore.Encryption.ValueConversion;

namespace Sourcey.EntityFrameworkCore.Extensions
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
