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

namespace Zion.EntityFrameworkCore.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreBuilder AddQueries(this IZionEntityFrameworkCoreBuilder builder, Action<DbContextOptionsBuilder> options, bool autoMigrate = true)
        {
            builder.Services.RemoveAll<IQueryStore>();
            builder.Services.TryAddScoped<IQueryStoreDbContextFactory, QueryStoreDbContextFactory>();
            builder.Services.TryAddSingleton<QueryStore>();
            builder.Services.TryAddSingleton<IQueryStore>(sp => sp.GetRequiredService<QueryStore>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<QueryStore>());
            builder.Services.AddDbContext<QueryStoreDbContext>(options);
            builder.Services.AddScoped<IZionInitializer, QueryStoreInitializer>();
            builder.Services.AddSingleton(new QueryStoreOptions(autoMigrate));
            return builder;
        }
    }
}
