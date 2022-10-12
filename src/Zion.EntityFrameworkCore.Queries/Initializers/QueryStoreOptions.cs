using Zion.EntityFrameworkCore.Queries.DbContexts;

namespace Zion.EntityFrameworkCore.Queries.Initializers
{
    internal sealed record QueryStoreOptions<TQueryStoreDbContext>(bool AutoMigrate)
        where TQueryStoreDbContext : QueryStoreDbContext;
}
