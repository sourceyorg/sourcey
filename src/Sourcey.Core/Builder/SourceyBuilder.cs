using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Core.Builder
{
    internal readonly struct SourceyBuilder : ISourceyBuilder
    {
        public readonly IServiceCollection Services { get; }

        public SourceyBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
