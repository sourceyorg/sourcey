using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.AWS.DynamoDB.Projections.Factories;

namespace Zion.AWS.DynamoDB.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection TryAddDynamoDBFactories(this IServiceCollection services)
        {
            services.TryAddSingleton<IDynamoDBOptionsFactory, DynamoDBOptionsFactory>();
            services.TryAddSingleton<IDynamoDBContextFactory, DynamoDBContextFactory>();
            services.TryAddSingleton<IDynamoDBClientFactory, DynamoDBClientFactory>();

            return services;
        }

        public static IServiceCollection AddDynamoDBOptions<TService>(this IServiceCollection services, Action<DynamoDBOptions> options)
        {
            var dynamoDBOptions = new DynamoDBOptions();
            options(dynamoDBOptions);

            services.AddSingleton(new DynamoDBConfiguration(typeof(TService), dynamoDBOptions));
            return services;
        }
    }
}
