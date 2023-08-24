using Microsoft.Extensions.Hosting;

namespace Sourcey.Events.Bus
{
    public interface IEventBusConsumer : IHostedService
    {
    }
}
