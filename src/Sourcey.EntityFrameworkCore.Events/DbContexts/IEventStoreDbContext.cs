using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.Entities;
using Sourcey.Events.Stores;

namespace Sourcey.EntityFrameworkCore.Events.DbContexts
{
    public interface IEventStoreDbContext : IEventStoreContext, IDisposable
    {
        DbSet<Event> Events { get; set; }
    }
}
