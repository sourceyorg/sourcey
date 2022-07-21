using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.DbContexts;

namespace Zion.EntityFrameworkCore.Events.Initializers
{
    internal record EventStoreInitializerOptions<TStoreDbContext>
        where TStoreDbContext : DbContext, IEventStoreDbContext
    {
        public readonly bool _autoMigrate;

        public EventStoreInitializerOptions(bool autoMigrate) => _autoMigrate = autoMigrate;

    }
}
