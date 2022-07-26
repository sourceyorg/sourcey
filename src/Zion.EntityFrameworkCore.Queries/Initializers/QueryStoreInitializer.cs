using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Queries.Factories;

namespace Zion.EntityFrameworkCore.Queries.Initializers
{
    internal class QueryStoreInitializer : IZionInitializer
    {
        public bool ParallelEnabled => false;
        private readonly IQueryStoreDbContextFactory _dbContextFactory;
        private readonly QueryStoreOptions _options;


        public QueryStoreInitializer(IQueryStoreDbContextFactory dbContextFactory,
            QueryStoreOptions options)
        {
            if (dbContextFactory is null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _dbContextFactory = dbContextFactory;
            _options = options;
        }

        public async Task InitializeAsync(IHost host)
        {
            if (!_options.AutoMigrate)
                return;

            using var context = _dbContextFactory.Create();
            await context.Database.MigrateAsync();
        }
    }
}
