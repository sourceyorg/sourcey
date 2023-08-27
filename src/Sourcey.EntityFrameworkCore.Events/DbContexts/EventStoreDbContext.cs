using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Events.DbContexts;

public class EventStoreDbContext : EventStoreDbContextBase<EventStoreDbContext>
{
    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
        : base(options)
    {
    }
}
