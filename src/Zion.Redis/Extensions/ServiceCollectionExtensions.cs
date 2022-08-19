using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Zion.Redis.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisConnectionFactory(this IServiceCollection services)
        {
            services.TryAddSingleton<IConnectionMultiplexerFactory, ConnectionMultiplexerFactory>();
            return services;
        }
    }
}
