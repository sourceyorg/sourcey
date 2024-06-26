using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;
using Sourcey.EntityFrameworkCore.Projections.Initializers;
using Sourcey.Initialization;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

internal readonly struct EntityFrameworkCoreProjectionWriterBuilder<TProjection> : IEntityFrameworkCoreProjectionWriterBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    private readonly IServiceCollection _services;

    public EntityFrameworkCoreProjectionWriterBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }

    public IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithContext<TProjectionContext>(bool autoMigrate = true)
        where TProjectionContext : DbContext
    {
        _services.AddSingleton(new WriteableProjectionDbType(typeof(TProjection), typeof(DbContextOptions<TProjectionContext>), typeof(TProjectionContext)));
        _services.AddScoped<ISourceyInitializer, ProjectionInitializer<TProjection>>();
        _services.AddSingleton(new ProjectionOptions<TProjection>(autoMigrate));

        _services.TryAddSingleton<IDbTypeFactory<WriteableProjectionDbType>, DbTypeFactory<WriteableProjectionDbType>>();
        _services.TryAddScoped<IWriteableProjectionDbContextFactory, WriteableProjectionDbContextFactory>();

        return this;
    }
}
