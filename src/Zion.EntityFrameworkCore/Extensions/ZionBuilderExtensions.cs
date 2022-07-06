using Zion.Core.Builder;
using Zion.EntityFrameworkCore.Builder;

namespace Zion.EntityFrameworkCore.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddEntityFrameworkCore(this IZionBuilder builder, Action<IZionEntityFrameworkCoreBuilder> configuration)
        {
            var entityFrameworkCoreBuilder = new ZionEntityFrameworkCoreBuilder(builder.Services);
            configuration(entityFrameworkCoreBuilder);
            return builder;
        }
    }
}
