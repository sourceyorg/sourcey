using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Initializers
{
    internal record ProjectionOptions<TProjection> 
        where TProjection : class, IProjection
    {
        public readonly bool _autoMigrate;

        public ProjectionOptions(bool autoMigrate) => _autoMigrate = autoMigrate;

    }
}
