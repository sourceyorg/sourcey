using Microsoft.Extensions.Hosting;

namespace Zion.Events.Bus
{
    public interface IEventBusConsumer : IHostedService
    {
    }
}
