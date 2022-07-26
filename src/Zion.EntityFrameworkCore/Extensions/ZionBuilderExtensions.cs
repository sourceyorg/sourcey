using Zion.Core.Builder;
using Zion.EntityFrameworkCore.Builder;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddEntityFrameworkCore(this IZionBuilder builder, Action<IZionEntityFrameworkCoreBuilder> configuration)
        {
            var entityFrameworkCoreBuilder = new ZionEntityFrameworkCoreBuilder(builder.Services);
            configuration(entityFrameworkCoreBuilder);
            return builder;
        }

        public static IZionBuilder AddEntityFrameworkCoreMigrator(this IZionBuilder builder, Action<IZionEntityFrameworkCoreMigratorBuilder> configuration)
        {
            var entityFrameworkCoreMigratorBuilder = new ZionEntityFrameworkCoreMigratorBuilder(builder.Services);
            configuration(entityFrameworkCoreMigratorBuilder);
            return builder;
        }
    }
}
