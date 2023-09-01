using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Sourcey.EntityFrameworkCore.Projections.Initializers;
using Sourcey.Initialization;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

internal readonly struct EntityFrameworkCoreProjectionManagerBuilder<TProjection> : IEntityFrameworkCoreProjectionManagerBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    private readonly IServiceCollection _services;

    public EntityFrameworkCoreProjectionManagerBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }

    public IEntityFrameworkCoreProjectionManagerBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
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
}