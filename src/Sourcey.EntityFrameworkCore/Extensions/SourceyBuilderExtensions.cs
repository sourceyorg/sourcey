using Sourcey.Builder;
using Sourcey.EntityFrameworkCore.Builder;

namespace Sourcey.Extensions;

public static class SourceyBuilderExtensions
{
    public static ISourceyBuilder AddEntityFrameworkCore(this ISourceyBuilder builder, Action<IEntityFrameworkCoreBuilder> configuration)
    {
        var entityFrameworkCoreBuilder = new EntityFrameworkCoreBuilder(builder.Services);
        configuration(entityFrameworkCoreBuilder);
        return builder;
    }

    public static ISourceyBuilder AddEntityFrameworkCoreMigrator(this ISourceyBuilder builder, Action<IEntityFrameworkCoreMigratorBuilder> configuration)
    {
        var entityFrameworkCoreMigratorBuilder = new EntityFrameworkCoreMigratorBuilder(builder.Services);
        configuration(entityFrameworkCoreMigratorBuilder);
        return builder;
    }
}
