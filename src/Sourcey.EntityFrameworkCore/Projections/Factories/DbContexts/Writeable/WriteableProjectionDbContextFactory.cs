using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;

internal sealed class WriteableProjectionDbContextFactory : DbContextFactory<DbContext, WriteableProjectionDbType>, IWriteableProjectionDbContextFactory
{
    public WriteableProjectionDbContextFactory(IServiceProvider serviceProvider, IDbTypeFactory<WriteableProjectionDbType> dbTypeFactory) : base(serviceProvider, dbTypeFactory)
    {
    }
}
