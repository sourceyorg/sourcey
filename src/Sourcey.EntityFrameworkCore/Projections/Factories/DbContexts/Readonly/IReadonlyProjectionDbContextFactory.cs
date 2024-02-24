
using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Readonly;

public interface IReadonlyProjectionDbContextFactory : DbContexts.IDbContextFactory<DbContext>
{
}
