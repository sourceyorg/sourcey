using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.EntityFrameworkCore.Projections.Builder;
using Zion.EntityFrameworkCore.Projections.Factories.DbContexts;
using Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
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
            builder.Services.TryAddScoped<IProjectionWriter<TProjection>, ProjectionWriter<TProjection>>();
            builder.Services.TryAddScoped<IProjectionReader<TProjection>, ProjectionReader<TProjection>>();

            var entityFrameworkCoreProjectionWriterBuilder = new EntityFrameworkCoreProjectionWriterBuilder<TProjection>(builder.Services);
            configuration(entityFrameworkCoreProjectionWriterBuilder);

            return builder;
        }
    }
}
