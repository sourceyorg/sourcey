using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Encryption;
using Sourcey.Encryption.Builder;

namespace Sourcey.Extensions
{
    public static class EncryptionBuilderExtensions
    {
        public static IEncryptionBuilder WithPassThrough(this IEncryptionBuilder builder)
        {
            builder.Services.TryAddScoped<IEncryptor, PassThroughEncryptor>();

            return builder;
        }
    }
}
