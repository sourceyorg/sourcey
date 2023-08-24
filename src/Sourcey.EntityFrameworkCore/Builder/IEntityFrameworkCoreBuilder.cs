using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.EntityFrameworkCore.Builder
{
    public interface IEntityFrameworkCoreBuilder
    {
        IServiceCollection Services { get; }
    }
}
