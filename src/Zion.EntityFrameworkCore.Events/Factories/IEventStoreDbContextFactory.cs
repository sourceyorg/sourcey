using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.DbContexts;

namespace Zion.EntityFrameworkCore.Events.Factories
{
    public interface IEventStoreDbContextFactory<TEventStoreDbContext>
        where TEventStoreDbContext : DbContext, IEventStoreDbContext
    {
        TEventStoreDbContext Create();
    }
}
