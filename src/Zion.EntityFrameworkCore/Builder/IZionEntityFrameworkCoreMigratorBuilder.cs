using Microsoft.EntityFrameworkCore;

namespace Zion.EntityFrameworkCore.Builder
{
    public interface IZionEntityFrameworkCoreMigratorBuilder
    {
        IZionEntityFrameworkCoreMigratorBuilder WithDbContext<TDbContext>(Action<DbContextOptionsBuilder> configuration)
            where TDbContext : DbContext;
    }
}
