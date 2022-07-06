using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zion.EntityFrameworkCore.Queries.DbContexts;

namespace Zion.EntityFrameworkCore.Queries.Factories
{
    internal sealed class QueryStoreDbContextFactory : IQueryStoreDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryStoreDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public QueryStoreDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<QueryStoreDbContext>>();
            return new QueryStoreDbContext(options);
        }
    }
}
