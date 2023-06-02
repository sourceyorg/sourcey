using Microsoft.Extensions.DependencyInjection;

namespace Zion.Files.Builder
{
    internal readonly struct ZionFilesBuilder : IZionFilesBuilder
    {
        public readonly IServiceCollection Services { get; }

        public ZionFilesBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
