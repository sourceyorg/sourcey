using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Azure.Encryption;
using Sourcey.Encryption;
using Sourcey.Encryption.Builder;

namespace Sourcey.Extensions
{
    public static class EncryptionBuilderExtensions
    {
        public static IEncryptionBuilder WithAzureKeyVault(this IEncryptionBuilder builder, Action<KeyVaultOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.TryAddScoped<IEncryptor, KeyVaultEncryptor>();

            return builder;
        }
    }
}
