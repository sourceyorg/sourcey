using Sourcey.Events;
using Sourcey.Events.Stores;

namespace Sourcey.EntityFrameworkCore.Events.Factories;

public interface IEventContextFactory
{
    IEventContext<IEvent> CreateContext(Entities.Event dbEvent);
}
