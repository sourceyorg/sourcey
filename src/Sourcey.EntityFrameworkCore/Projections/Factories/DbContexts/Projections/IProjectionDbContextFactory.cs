
using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;

public interface IProjectionDbContextFactory : DbContexts.IDbContextFactory<DbContext>
{
}
