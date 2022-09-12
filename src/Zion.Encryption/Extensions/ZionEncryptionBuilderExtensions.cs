using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Encryption;
using Zion.Encryption.Builder;

namespace Zion.Extensions
{
    public static class ZionEncryptionBuilderExtensions
    {
        public static IZionEncryptionBuilder WithPassThrough(this IZionEncryptionBuilder builder)
        {
            builder.Services.TryAddScoped<IEncryptor, PassThroughEncryptor>();

            return builder;
        }
    }
}
