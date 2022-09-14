using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Azure.Encryption;
using Zion.Encryption;
using Zion.Encryption.Builder;

namespace Zion.Extensions
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
