using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates;
using Sourcey.EntityFrameworkCore.Projections.Initializers;
using Sourcey.Initialization;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

internal readonly struct EntityFrameworkCoreStateManagementBuilder<TProjection> : IEntityFrameworkCoreStateManagementBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    private readonly IServiceCollection _services;

    public EntityFrameworkCoreStateManagementBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }

    public IEntityFrameworkCoreStateManagementBuilder<TProjection> WithContext<TProjectionStateDbContext>(bool autoMigrate = true)
        where TProjectionStateDbContext : ProjectionStateDbContext
    {
        _services.AddSingleton(new ProjectionStateDbType(typeof(TProjection), typeof(DbContextOptions<TProjectionStateDbContext>), typeof(TProjectionStateDbContext)));
        _services.AddSingleton(new ProjectionStateOptions<TProjection>(autoMigrate));
        
        _services.TryAddSingleton<IDbTypeFactory<ProjectionStateDbType>, DbTypeFactory<ProjectionStateDbType>>();
        _services.AddScoped<ISourceyInitializer, ProjectionStateInitializer<TProjection>>();
        _services.TryAddScoped<IProjectionStateManager<TProjection>, ProjectionStateManager<TProjection>>();
        _services.TryAddScoped<IProjectionStateDbContextFactory, ProjectionStateDbContextFactory>();

        return this;
    }
}
