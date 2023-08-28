using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.EntityFrameworkCore.Projections;
using Sourcey.EntityFrameworkCore.Projections.Builder;
using Sourcey.Projections;
using Sourcey.Projections.Builder;

namespace Sourcey.Extensions;

public static class ProjectionBuilderExtensions
{
    public static IProjectionBuilder<TProjection> WithEntityFrameworkCoreWriter<TProjection>(this IProjectionBuilder<TProjection> builder,
        Action<IEntityFrameworkCoreProjectionWriterBuilder<TProjection>> configuration)
        
        where TProjection : class, IProjection
    {
        builder.Services.TryAddScoped<IProjectionWriter<TProjection>, ProjectionWriter<TProjection>>();
        builder.Services.TryAddScoped<IProjectionReader<TProjection>, ProjectionReader<TProjection>>();

        var entityFrameworkCoreProjectionWriterBuilder = new EntityFrameworkCoreProjectionWriterBuilder<TProjection>(builder.Services);
        configuration(entityFrameworkCoreProjectionWriterBuilder);

        return builder;
    }
}
