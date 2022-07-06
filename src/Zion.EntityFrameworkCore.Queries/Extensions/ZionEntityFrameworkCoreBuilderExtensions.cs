using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Queries.DbContexts;
using Zion.EntityFrameworkCore.Queries.Factories;
using Zion.EntityFrameworkCore.Queries.Stores;
using Zion.Queries.Stores;

namespace Zion.EntityFrameworkCore.Queries.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreBuilder AddQueries(this IZionEntityFrameworkCoreBuilder builder, Action<DbContextOptionsBuilder> options)
        {
            builder.Services.RemoveAll<IQueryStore>();
            builder.Services.TryAdd(GetQueryServices());
            builder.Services.AddDbContext<QueryStoreDbContext>(options);
            return builder;
        }

        private static IEnumerable<ServiceDescriptor> GetQueryServices()
        {
            yield return ServiceDescriptor.Scoped<IQueryStoreDbContextFactory, QueryStoreDbContextFactory>();
            yield return ServiceDescriptor.Scoped<IQueryStore, QueryStore>();
        }
    }
}
