using Microsoft.Extensions.Hosting;

namespace Sourcey.Initialization;

public interface ISourceyInitializer
{
    public bool ParallelEnabled { get; }

    Task InitializeAsync(IHost host);
}
