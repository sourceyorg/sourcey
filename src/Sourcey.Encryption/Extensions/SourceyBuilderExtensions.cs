using Sourcey.Core.Builder;
using Sourcey.Encryption.Builder;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddEncryption(this ISourceyBuilder builder, Action<IEncryptionBuilder> configure)
        {
            var encryptionBuilder = new EncryptionBuilder(builder.Services);
            configure(encryptionBuilder);
            return builder;
        }
    }
}
