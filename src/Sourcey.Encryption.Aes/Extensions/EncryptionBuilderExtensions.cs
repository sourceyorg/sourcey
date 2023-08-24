using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Encryption;
using Sourcey.Encryption.Aes;
using Sourcey.Encryption.Builder;

namespace Sourcey.Extensions
{
    public static class EncryptionBuilderExtensions
    {
        public static IEncryptionBuilder WithAes(this IEncryptionBuilder builder, Action<AesOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.TryAddScoped<IEncryptor, AesEncryptor>();

            return builder;
        }
    }
}
