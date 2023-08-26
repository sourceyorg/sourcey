using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Core.Initialization;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates;
using Sourcey.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Sourcey.EntityFrameworkCore.Projections.Initializers;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Builder;

internal readonly struct EntityFrameworkCoreProjectionWriterBuilder<TProjection> : IEntityFrameworkCoreProjectionWriterBuilder<TProjection>
    where TProjection : class, IProjection
{
    private readonly IServiceCollection _services;

    public EntityFrameworkCoreProjectionWriterBuilder(IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }

    public IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
        where TProjectionContext : DbContext
    {
        _services.AddSingleton(new ProjectionDbType(typeof(TProjection), typeof(DbContextOptions<TProjectionContext>), typeof(TProjectionContext)));
        _services.AddScoped<ISourceyInitializer, ProjectionInitializer<TProjection>>();
        _services.AddSingleton(new ProjectionOptions<TProjection>(autoMigrate));
        _services.AddDbContext<TProjectionContext>(dbOptions);

        _services.TryAddSingleton<IDbTypeFactory<ProjectionDbType>, DbTypeFactory<ProjectionDbType>>();
        _services.TryAddScoped<IProjectionDbContextFactory, ProjectionDbContextFactory>();

        return this;
    }

    public IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithStateManagement<TProjectionStateDbContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
        where TProjectionStateDbContext : ProjectionStateDbContext
    {
        _services.AddDbContext<TProjectionStateDbContext>(dbOptions);
        _services.AddSingleton(new ProjectionStateDbType(typeof(TProjection), typeof(DbContextOptions<TProjectionStateDbContext>), typeof(TProjectionStateDbContext)));
        _services.AddSingleton(new ProjectionStateOptions<TProjection>(autoMigrate));
        
        _services.TryAddSingleton<IDbTypeFactory<ProjectionStateDbType>, DbTypeFactory<ProjectionStateDbType>>();
        _services.AddScoped<ISourceyInitializer, ProjectionStateInitializer<TProjection>>();
        _services.TryAddScoped<IProjectionStateManager<TProjection>, ProjectionStateManager<TProjection>>();
        _services.TryAddScoped<IProjectionStateDbContextFactory, ProjectionStateDbContextFactory>();

        return this;
    }
}
