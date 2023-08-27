using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Initializers;

internal sealed record ProjectionOptions<TProjection>(bool AutoMigrate)
    where TProjection : class, IProjection;
