using Microsoft.Extensions.DependencyInjection;

namespace Zion.Files.Builder
{
    public interface IZionFilesBuilder
    {
        IServiceCollection Services { get; }
    }
}
