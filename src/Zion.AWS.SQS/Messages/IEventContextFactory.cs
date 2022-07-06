using Amazon.SQS.Model;
using Zion.Events;
using Zion.Events.Stores;

namespace Zion.AWS.SQS.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Message message);
    }
}
