using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Readonly;

internal sealed class ReadonlyProjectionDbContextFactory : DbContextFactory<DbContext, ReadonlyProjectionDbType>, IReadonlyProjectionDbContextFactory
{
    public ReadonlyProjectionDbContextFactory(IServiceProvider serviceProvider, IDbTypeFactory<ReadonlyProjectionDbType> dbTypeFactory) : base(serviceProvider, dbTypeFactory)
    {
    }
}
