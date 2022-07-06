using Zion.EntityFrameworkCore.Queries.DbContexts;

namespace Zion.EntityFrameworkCore.Queries.Factories
{
    public interface IQueryStoreDbContextFactory
    {
        QueryStoreDbContext Create();
    }
}
