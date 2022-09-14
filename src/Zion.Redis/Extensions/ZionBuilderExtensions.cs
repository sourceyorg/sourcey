using Zion.Core.Builder;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddRedis(this IZionBuilder builder)
        {
            builder.Services.AddRedisConnectionFactory();
            return builder;
        }
    }
}
