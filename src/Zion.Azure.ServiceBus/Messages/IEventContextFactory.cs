using Microsoft.Azure.ServiceBus;
using Zion.Events;
using Zion.Events.Stores;

namespace Zion.Azure.ServiceBus.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Message message);
    }
}
