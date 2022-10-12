using Microsoft.EntityFrameworkCore;

namespace Zion.EntityFrameworkCore.Queries.DbContexts
{
    public sealed class DefaultQueryStoreDbContext : QueryStoreDbContext
    {
        public DefaultQueryStoreDbContext(DbContextOptions<DefaultQueryStoreDbContext> options) : base(options)
        {
        }
    }
}
