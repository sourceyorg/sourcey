using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.DbContexts;

namespace Zion.EntityFrameworkCore.Events.Initializers
{
    internal sealed record EventStoreInitializerOptions<TStoreDbContext>(bool AutoMigrate)
        where TStoreDbContext : DbContext, IEventStoreDbContext;
}
