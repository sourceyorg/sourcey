using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Queries.DbContexts;

namespace Zion.EntityFrameworkCore.Queries.Initializers
{
    internal sealed class QueryStoreInitializer : IZionInitializer
    {
        public bool ParallelEnabled => false;
        private readonly QueryStoreOptions _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public QueryStoreInitializer(IServiceScopeFactory serviceScopeFactory,
            QueryStoreOptions options)
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
            using var context = scope.ServiceProvider.GetRequiredService<QueryStoreDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
