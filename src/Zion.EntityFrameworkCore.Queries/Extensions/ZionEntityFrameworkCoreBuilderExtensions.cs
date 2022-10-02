using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Queries.DbContexts;
using Zion.EntityFrameworkCore.Queries.Factories;
using Zion.EntityFrameworkCore.Queries.Initializers;
using Zion.EntityFrameworkCore.Queries.Stores;
using Zion.Queries.Stores;
using BufferedQueryStore = Zion.EntityFrameworkCore.Queries.Stores.BufferedQueryStore;

namespace Zion.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreBuilder AddQueries(this IZionEntityFrameworkCoreBuilder builder, Action<DbContextOptionsBuilder> options, QueryStoreType type = QueryStoreType.Buffered, bool autoMigrate = true)
        {
            builder.Services.RemoveAll<IQueryStore>();
            builder.Services.TryAddScoped<IQueryStoreDbContextFactory, QueryStoreDbContextFactory>();
            builder.Services.AddDbContext<QueryStoreDbContext>(options);
            builder.Services.AddScoped<IZionInitializer, QueryStoreInitializer>();
            builder.Services.AddSingleton(new QueryStoreOptions(autoMigrate));

            if (type == QueryStoreType.Buffered)
                builder.RegisterBufferedQueryStore();
            else
                builder.Services.TryAddSingleton<IQueryStore, QueryStore>();
            
            return builder;
        }

        private static void RegisterBufferedQueryStore(this IZionEntityFrameworkCoreBuilder builder)
        {
            builder.Services.TryAddSingleton<BufferedQueryStore>();
            builder.Services.TryAddSingleton<IQueryStore>(sp => sp.GetRequiredService<BufferedQueryStore>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<BufferedQueryStore>());
        }
    }
}
