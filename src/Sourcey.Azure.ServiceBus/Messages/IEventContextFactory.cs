using Microsoft.Azure.ServiceBus;
using Sourcey.Events;
using Sourcey.Events.Stores;

namespace Sourcey.Azure.ServiceBus.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Message message);
    }
}
