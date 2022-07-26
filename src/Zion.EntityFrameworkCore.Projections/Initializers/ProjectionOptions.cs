using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Initializers
{
    internal sealed record ProjectionOptions<TProjection>(bool AutoMigrate)
        where TProjection : class, IProjection;
}
