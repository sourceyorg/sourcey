using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Readonly;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

internal readonly struct EntityFrameworkCoreProjectionReaderBuilder<TProjection> : IEntityFrameworkCoreProjectionReaderBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    private readonly IServiceCollection _services;

    public EntityFrameworkCoreProjectionReaderBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }

    public IEntityFrameworkCoreProjectionReaderBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions)
        where TProjectionContext : DbContext
    {
        _services.AddSingleton(new ReadonlyProjectionDbType(typeof(TProjection), typeof(DbContextOptions<TProjectionContext>), typeof(TProjectionContext)));
        _services.AddDbContextPool<TProjectionContext>(dbOptions);
        _services.TryAddSingleton<IDbTypeFactory<ReadonlyProjectionDbType>, DbTypeFactory<ReadonlyProjectionDbType>>();
        _services.TryAddScoped<IReadonlyProjectionDbContextFactory, ReadonlyProjectionDbContextFactory>();

        return this;
    }
}
