using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.Entities;
using Zion.Events.Stores;

namespace Zion.EntityFrameworkCore.Events.DbContexts
{
    public interface IEventStoreDbContext : IEventStoreContext, IDisposable
    {
        DbSet<Event> Events { get; set; }
    }
}
