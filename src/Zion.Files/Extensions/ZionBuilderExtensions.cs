using Zion.Core.Builder;
using Zion.Files.Builder;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddFiles(this IZionBuilder builder, Action<IZionFilesBuilder> configure)
        {
            var encryptionBuilder = new ZionFilesBuilder(builder.Services);
            configure(encryptionBuilder);
            return builder;
        }
    }
}
