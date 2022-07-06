using Zion.Core.Builder;
using Zion.Encryption.Builder;

namespace Zion.Encryption.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddEncryption(IZionBuilder builder, Action<IZionEncryptionBuilder> configure)
        {
            var encryptionBuilder = new ZionEncryptionBuilder(builder.Services);
            configure(encryptionBuilder);
            return builder;
        }
    }
}
