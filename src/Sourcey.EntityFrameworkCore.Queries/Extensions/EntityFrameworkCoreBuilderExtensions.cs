using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Core.Initialization;
using Sourcey.EntityFrameworkCore.Builder;
using Sourcey.EntityFrameworkCore.Queries.DbContexts;
using Sourcey.EntityFrameworkCore.Queries.Initializers;
using Sourcey.EntityFrameworkCore.Queries.Stores;
using Sourcey.Queries.Stores;

namespace Sourcey.Extensions
{
    public static class EntityFrameworkCoreBuilderExtensions
    {
        public static IEntityFrameworkCoreBuilder AddQueries(
            this IEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            QueryStoreType type = QueryStoreType.Buffered,
            bool autoMigrate = true)
            => AddQueries<DefaultQueryStoreDbContext>(builder, options, type, autoMigrate);

        public static IEntityFrameworkCoreBuilder AddQueries<TQueryStoreDbContext>(
            this IEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            QueryStoreType type = QueryStoreType.Buffered,
            bool autoMigrate = true)
            where TQueryStoreDbContext : QueryStoreDbContext
        {
            builder.Services.RemoveAll<IQueryStore<TQueryStoreDbContext>>();
            builder.Services.AddDbContext<TQueryStoreDbContext>(options);
            builder.Services.AddScoped<ISourceyInitializer, QueryStoreInitializer<TQueryStoreDbContext>>();
            builder.Services.AddSingleton(new QueryStoreOptions<TQueryStoreDbContext>(autoMigrate));

            if (type == QueryStoreType.Buffered)
                builder.RegisterBufferedQueryStore<TQueryStoreDbContext>();
            else
                builder.Services.TryAddSingleton<IQueryStore<TQueryStoreDbContext>, QueryStore<TQueryStoreDbContext>>();

            return builder;
        }

        private static void RegisterBufferedQueryStore<TQueryStoreDbContext>(this IEntityFrameworkCoreBuilder builder)
            where TQueryStoreDbContext : QueryStoreDbContext
        {
            builder.Services.TryAddSingleton<EntityFrameworkCore.Queries.Stores.BufferedQueryStore<TQueryStoreDbContext>>();
            builder.Services.TryAddSingleton<IQueryStore<TQueryStoreDbContext>>(sp => sp.GetRequiredService<EntityFrameworkCore.Queries.Stores.BufferedQueryStore<TQueryStoreDbContext>>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<EntityFrameworkCore.Queries.Stores.BufferedQueryStore<TQueryStoreDbContext>>());
        }
    }
}
