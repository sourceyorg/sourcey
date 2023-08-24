using Sourcey.EntityFrameworkCore.Queries.DbContexts;

namespace Sourcey.EntityFrameworkCore.Queries.Initializers
{
    internal sealed record QueryStoreOptions<TQueryStoreDbContext>(bool AutoMigrate)
        where TQueryStoreDbContext : QueryStoreDbContext;
}
