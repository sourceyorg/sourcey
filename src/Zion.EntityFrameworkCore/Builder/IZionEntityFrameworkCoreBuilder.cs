using Microsoft.Extensions.DependencyInjection;

namespace Zion.EntityFrameworkCore.Builder
{
    public interface IZionEntityFrameworkCoreBuilder
    {
        IServiceCollection Services { get; }
    }
}
