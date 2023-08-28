using Microsoft.Extensions.Hosting;

namespace Sourcey.Projections;

public interface IProjector<TProcess> : IHostedService, IDisposable
{
    Task ResetAsync(CancellationToken cancellationToken = default);
}
