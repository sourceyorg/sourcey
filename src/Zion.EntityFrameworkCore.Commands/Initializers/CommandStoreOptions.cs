using Zion.EntityFrameworkCore.Commands.DbContexts;

namespace Zion.EntityFrameworkCore.Commands.Initializers
{
    internal sealed record CommandStoreOptions<TCommandStoreDbContext>(bool AutoMigrate)
        where TCommandStoreDbContext : CommandStoreDbContext;
}
