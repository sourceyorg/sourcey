using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Initializers
{
    internal record ProjectionStateOptions<TProjection> 
        where TProjection : class, IProjection
    {
        public readonly bool _autoMigrate;

        public ProjectionStateOptions(bool autoMigrate) => _autoMigrate = autoMigrate;

    }
}
