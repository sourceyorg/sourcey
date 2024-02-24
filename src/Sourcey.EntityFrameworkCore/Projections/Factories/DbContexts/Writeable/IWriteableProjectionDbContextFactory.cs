
using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;

public interface IWriteableProjectionDbContextFactory : DbContexts.IDbContextFactory<DbContext>
{
}
