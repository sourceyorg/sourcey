using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Encryption.Builder;

namespace Zion.Encryption.Aes.Extensions
{
    public static class ZionEncryptionBuilderExtensions
    {
        public static IZionEncryptionBuilder WithAes(this IZionEncryptionBuilder builder, Action<AesOptions> options)
        {
            builder.Services.Configure(options);
            builder.Services.TryAddScoped<IEncryptor, AesEncryptor>();

            return builder;
        }
    }
}
