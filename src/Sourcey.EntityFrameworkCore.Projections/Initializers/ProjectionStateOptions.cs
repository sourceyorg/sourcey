using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Initializers;

internal sealed record ProjectionStateOptions<TProjection>(bool AutoMigrate) 
    where TProjection : class, IProjection;
