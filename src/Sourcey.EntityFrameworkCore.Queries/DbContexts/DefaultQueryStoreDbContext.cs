using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Queries.DbContexts
{
    public sealed class DefaultQueryStoreDbContext : QueryStoreDbContext
    {
        public DefaultQueryStoreDbContext(DbContextOptions<DefaultQueryStoreDbContext> options) : base(options)
        {
        }
    }
}
