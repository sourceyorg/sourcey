using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sourcey.Projections;
using Sourcey.Projections.Builder;
using Sourcey.Projections.InMemory;

namespace Sourcey.Extensions;

public static class ProjectionBuilderExtensions
{
    public static IProjectionBuilder<TProjection> WithInMemoryStateManager<TProjection>(this IProjectionBuilder<TProjection> builder)
        where TProjection : class, IProjection, new()
    {
        builder.Services.TryAddSingleton<IProjectionStateManager<TProjection>, InMemoryProjectionStateManager<TProjection>>();
        return builder;
    }

    public static IProjectionBuilder<TProjection> WithInMemoryWriter<TProjection>(this IProjectionBuilder<TProjection> builder)
        where TProjection : class, IProjection, new()
    {
        builder.Services.AddSingleton<InMemoryProjectionManager<TProjection>>();
        builder.Services.AddSingleton<IProjectionWriter<TProjection>>(sp => sp.GetRequiredService<InMemoryProjectionManager<TProjection>>());
        builder.Services.AddSingleton<IProjectionReader<TProjection>>(sp => sp.GetRequiredService<InMemoryProjectionManager<TProjection>>());
        return builder;
    }
}