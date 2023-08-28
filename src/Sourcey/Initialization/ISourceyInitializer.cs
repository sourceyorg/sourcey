using Microsoft.Extensions.Hosting;

namespace Sourcey.Core.Initialization;

public interface ISourceyInitializer
{
    public bool ParallelEnabled { get; }

    Task InitializeAsync(IHost host);
}
