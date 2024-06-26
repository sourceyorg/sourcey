using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.EntityFrameworkCore.Projections;
using Sourcey.Projections;
using Sourcey.Projections.Builder;

namespace Sourcey.Extensions;

public static partial class ProjectionBuilderExtensions
{
    public static IProjectionBuilder<TProjection> WithEntityFrameworkCoreStateManager<TProjection>(
        this IProjectionBuilder<TProjection> builder,
        Action<IEntityFrameworkCoreStateManagementBuilder<TProjection>> action)
        where TProjection : class, IProjection, new()
    {
        builder.Services.TryAddScoped<IProjectionStateManager<TProjection>, ProjectionStateManager<TProjection>>();
        action(new EntityFrameworkCoreStateManagementBuilder<TProjection>(builder.Services));
        return builder;
    }

    public static IProjectionBuilder<TProjection> WithEntityFrameworkCoreWriter<TProjection>(this IProjectionBuilder<TProjection> builder, Action<IEntityFrameworkCoreProjectionWriterBuilder<TProjection>> action)
        where TProjection : class, IProjection, new()
    {
        builder.Services.AddScoped<IProjectionWriter<TProjection>, ProjectionWriter<TProjection>>();
        action(new EntityFrameworkCoreProjectionWriterBuilder<TProjection>(builder.Services));
        return builder;
    }
    
    public static IProjectionBuilder<TProjection> WithEntityFrameworkCoreReader<TProjection>(this IProjectionBuilder<TProjection> builder, Action<IEntityFrameworkCoreProjectionReaderBuilder<TProjection>> action)
        where TProjection : class, IProjection, new()
    {
        builder.Services.AddScoped<IProjectionReader<TProjection>, ProjectionReader<TProjection>>();
        action(new EntityFrameworkCoreProjectionReaderBuilder<TProjection>(builder.Services));
        return builder;
    }
}
