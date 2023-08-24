using Sourcey.Core.Builder;
using Sourcey.Files.Builder;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddFiles(this ISourceyBuilder builder, Action<IFilesBuilder> configure)
        {
            var filesBuilder = new FilesBuilder(builder.Services);
            configure(filesBuilder);
            return builder;
        }
    }
}
