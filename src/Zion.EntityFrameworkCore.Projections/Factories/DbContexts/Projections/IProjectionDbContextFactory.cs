
using Microsoft.EntityFrameworkCore;

namespace Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts
{
    public interface IProjectionDbContextFactory : DbContexts.IDbContextFactory<DbContext>
    {
    }
}
