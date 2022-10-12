using Microsoft.EntityFrameworkCore;

namespace Zion.EntityFrameworkCore.Commands.DbContexts
{
    public sealed class DefaultCommandStoreDbContext : CommandStoreDbContext
    {
        public DefaultCommandStoreDbContext(DbContextOptions<DefaultCommandStoreDbContext> options) : base(options)
        {
        }
    }
}
