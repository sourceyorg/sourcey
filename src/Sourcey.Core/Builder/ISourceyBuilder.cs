using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Core.Builder
{
    public interface ISourceyBuilder
    {
        IServiceCollection Services { get; }
    }
}
