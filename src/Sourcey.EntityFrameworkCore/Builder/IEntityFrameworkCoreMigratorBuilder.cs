using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Builder
{
    public interface IEntityFrameworkCoreMigratorBuilder
    {
        IEntityFrameworkCoreMigratorBuilder WithDbContext<TDbContext>(Action<DbContextOptionsBuilder> configuration)
            where TDbContext : DbContext;
    }
}
