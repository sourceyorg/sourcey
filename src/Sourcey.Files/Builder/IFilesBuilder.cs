using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Files.Builder
{
    public interface IFilesBuilder
    {
        IServiceCollection Services { get; }
    }
}
