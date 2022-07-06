using Microsoft.Extensions.DependencyInjection;

namespace Zion.Core.Builder
{
    public interface IZionBuilder
    {
        IServiceCollection Services { get; }
    }
}
