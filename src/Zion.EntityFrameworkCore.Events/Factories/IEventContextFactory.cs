using Zion.Events;
using Zion.Events.Stores;

namespace Zion.EntityFrameworkCore.Events.Factories
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Entities.Event dbEvent);
    }
}
