using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.AWS.DynamoDB.Extensions;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.AWS.DynamoDB.Projections.Initializers;
using Zion.Core.Initialization;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDynamoDBWriter<TProjection>(this IServiceCollection services, Action<DynamoDBOptions> options)
            where TProjection : class, IProjection
        {
            services.TryAddDynamoDBFactories();
            services.TryAddSingleton<IProjectionTableInitializer, ProjectionInitializer>();
            services.AddDynamoDBOptions<TProjection>(options);
            services.AddSingleton<IProjectionReader<TProjection>, ProjectionReader<TProjection>>();
            services.AddSingleton<IProjectionWriter<TProjection>, ProjectionWriter<TProjection>>();
            services.AddSingleton<IProjectionStateManager<TProjection>, ProjectionStateManager<TProjection>>();
            services.AddSingleton<IZionInitializer, ProjectionInitializer<TProjection>>();

            return services;
        }
    }
}
