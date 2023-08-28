using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.DbContexts;

namespace Sourcey.EntityFrameworkCore.Events.Factories;

public interface IEventStoreDbContextFactory<TEventStoreDbContext>
    where TEventStoreDbContext : DbContext, IEventStoreDbContext
{
    TEventStoreDbContext Create();
}
