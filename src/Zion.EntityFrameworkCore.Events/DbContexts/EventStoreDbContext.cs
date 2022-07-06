using Microsoft.EntityFrameworkCore;

namespace Zion.EntityFrameworkCore.Events.DbContexts
{
    public class EventStoreDbContext : EventStoreDbContextBase<EventStoreDbContext>
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        {
        }
    }
}
