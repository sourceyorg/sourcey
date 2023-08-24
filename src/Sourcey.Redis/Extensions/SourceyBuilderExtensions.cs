using Sourcey.Core.Builder;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddRedis(this ISourceyBuilder builder)
        {
            builder.Services.AddRedisConnectionFactory();
            return builder;
        }
    }
}
