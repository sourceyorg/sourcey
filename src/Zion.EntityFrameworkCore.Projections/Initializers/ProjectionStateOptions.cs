using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Initializers
{
    internal sealed record ProjectionStateOptions<TProjection>(bool AutoMigrate) 
        where TProjection : class, IProjection;
}
