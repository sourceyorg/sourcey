using Sourcey.EntityFrameworkCore.Commands.DbContexts;

namespace Sourcey.EntityFrameworkCore.Commands.Initializers
{
    internal sealed record CommandStoreOptions<TCommandStoreDbContext>(bool AutoMigrate)
        where TCommandStoreDbContext : CommandStoreDbContext;
}
