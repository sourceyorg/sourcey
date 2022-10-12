using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Queries.DbContexts;
using Zion.EntityFrameworkCore.Queries.Initializers;
using Zion.EntityFrameworkCore.Queries.Stores;
using Zion.Queries.Stores;

namespace Zion.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreBuilder AddQueries(
            this IZionEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            QueryStoreType type = QueryStoreType.Buffered,
            bool autoMigrate = true)
            => AddQueries<DefaultQueryStoreDbContext>(builder, options, type, autoMigrate);

        public static IZionEntityFrameworkCoreBuilder AddQueries<TQueryStoreDbContext>(
            this IZionEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            QueryStoreType type = QueryStoreType.Buffered,
            bool autoMigrate = true)
            where TQueryStoreDbContext : QueryStoreDbContext
        {
            builder.Services.RemoveAll<IQueryStore<TQueryStoreDbContext>>();
            builder.Services.AddDbContext<TQueryStoreDbContext>(options);
            builder.Services.AddScoped<IZionInitializer, QueryStoreInitializer<TQueryStoreDbContext>>();
            builder.Services.AddSingleton(new QueryStoreOptions<TQueryStoreDbContext>(autoMigrate));

            if (type == QueryStoreType.Buffered)
                builder.RegisterBufferedQueryStore<TQueryStoreDbContext>();
            else
                builder.Services.TryAddSingleton<IQueryStore<TQueryStoreDbContext>, QueryStore<TQueryStoreDbContext>>();

            return builder;
        }

        private static void RegisterBufferedQueryStore<TQueryStoreDbContext>(this IZionEntityFrameworkCoreBuilder builder)
            where TQueryStoreDbContext : QueryStoreDbContext
        {
            builder.Services.TryAddSingleton<EntityFrameworkCore.Queries.Stores.BufferedQueryStore<TQueryStoreDbContext>>();
            builder.Services.TryAddSingleton<IQueryStore<TQueryStoreDbContext>>(sp => sp.GetRequiredService<EntityFrameworkCore.Queries.Stores.BufferedQueryStore<TQueryStoreDbContext>>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<EntityFrameworkCore.Queries.Stores.BufferedQueryStore<TQueryStoreDbContext>>());
        }
    }
}
