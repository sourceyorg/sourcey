using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Files.Builder
{
    internal readonly struct FilesBuilder : IFilesBuilder
    {
        public readonly IServiceCollection Services { get; }

        public FilesBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
