using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.EntityFrameworkCore.Projections.DbContexts;
using Zion.EntityFrameworkCore.Projections.Factories;
using Zion.Projections;
using Zion.Projections.Builder;

namespace Zion.EntityFrameworkCore.Projections.Extensions
{
    public static class ZionProjectionBuilderExtensions
    {
        public static IZionProjectionBuilder<TProjection> WithEntityFrameworkCoreWriter<TProjection>(this IZionProjectionBuilder<TProjection> builder,
            Action<IEntityFrameworkCoreProjectionWriterBuilder<TProjection>> configuration)
            
            where TProjection : class, IProjection
        {
            builder.Services.TryAdd(GetFactoryServices());
            
            builder.Services.TryAddScoped<IProjectionWriter<TProjection>, ProjectionWriter<TProjection>>();
            builder.Services.TryAddScoped<IProjectionReader<TProjection>, ProjectionReader<TProjection>>();
            builder.Services.TryAddScoped<IProjectionStateManager<TProjection>, ProjectionStateManager<TProjection>>();

            var entityFrameworkCoreProjectionWriterBuilder = new EntityFrameworkCoreProjectionWriterBuilder<TProjection>(builder.Services);
            configuration(entityFrameworkCoreProjectionWriterBuilder);

            return builder;
        }

        public static IEnumerable<ServiceDescriptor> GetFactoryServices()
        {
            yield return ServiceDescriptor.Scoped<IProjectionDbContextFactory, ProjectionDbContextFactory>();
            yield return ServiceDescriptor.Singleton<IProjectionDbContextTypesFactory, ProjectionDbContextTypesFactory>();
        }
    }

    public interface IEntityFrameworkCoreProjectionWriterBuilder<TProjection>
        where TProjection : class, IProjection
    {
        IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions)
            where TProjectionContext : ProjectionDbContext;
    }

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
        
        public IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions)
            where TProjectionContext : ProjectionDbContext            
        {
            _services.AddSingleton(new ProjectionDbTypes(typeof(TProjection), typeof(DbContextOptions<TProjectionContext>), typeof(TProjectionContext)));
            _services.AddDbContext<TProjectionContext>(dbOptions);

            return this;
        }
    }
}
