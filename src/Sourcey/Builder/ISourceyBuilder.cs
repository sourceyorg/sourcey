using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Builder;

public interface ISourceyBuilder
{
    IServiceCollection Services { get; }
}
