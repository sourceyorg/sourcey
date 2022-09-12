using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Encryption;
using Zion.Encryption.Builder;

namespace Zion.Azure.Encryption.Extensions
{
    public static class ZionEncryptionBuilderExtensions
    {
        public static IZionEncryptionBuilder WithAzureKeyVault(this IZionEncryptionBuilder builder, Action<KeyVaultOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.TryAddScoped<IEncryptor, KeyVaultEncryptor>();

            return builder;
        }
    }
}
