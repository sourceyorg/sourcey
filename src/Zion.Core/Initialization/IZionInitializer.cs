using Microsoft.Extensions.Hosting;

namespace Zion.Core.Initialization
{
    public interface IZionInitializer
    {
        public bool ParallelEnabled { get; }

        Task InitializeAsync(IHost host);
    }
}
