using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sourcey.Core.Initialization;
using Sourcey.EntityFrameworkCore.Queries.DbContexts;

namespace Sourcey.EntityFrameworkCore.Queries.Initializers
{
    internal sealed class QueryStoreInitializer<TQueryStoreDbContext> : ISourceyInitializer
        where TQueryStoreDbContext : QueryStoreDbContext
    {
        public bool ParallelEnabled => false;
        private readonly QueryStoreOptions<TQueryStoreDbContext> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public QueryStoreInitializer(IServiceScopeFactory serviceScopeFactory,
            QueryStoreOptions<TQueryStoreDbContext> options)
        {
            if (serviceScopeFactory is null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
        }

        public async Task InitializeAsync(IHost host)
        {
            if (!_options.AutoMigrate)
                return;

            using var scope = _serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<TQueryStoreDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
