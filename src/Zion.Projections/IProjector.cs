using Microsoft.Extensions.Hosting;

namespace Zion.Projections
{
    public interface IProjector<TProcess> : IHostedService, IDisposable
    {
        Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
