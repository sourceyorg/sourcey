using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.DbContexts;

public sealed class DefaultProjectionStateDbContext : ProjectionStateDbContext
{
    public DefaultProjectionStateDbContext(DbContextOptions<DefaultProjectionStateDbContext> options) : base(options)
    {
    }
}
