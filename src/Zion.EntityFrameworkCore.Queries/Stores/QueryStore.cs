using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zion.EntityFrameworkCore.Queries.Entities;
using Zion.EntityFrameworkCore.Queries.Factories;
using Zion.Queries;
using Zion.Queries.Serialization;
using Zion.Queries.Stores;

namespace Zion.EntityFrameworkCore.Queries.Stores
{
    internal sealed class QueryStore : IQueryStore
    {
        private readonly IQueryStoreDbContextFactory _dbContextFactory;
        private readonly IQuerySerializer _querySerializer;
        private readonly ILogger<QueryStore> _logger;

        public QueryStore(IQueryStoreDbContextFactory dbContextFactory,
            IQuerySerializer querySerializer,
            ILogger<QueryStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (querySerializer == null)
                throw new ArgumentNullException(nameof(querySerializer));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _querySerializer = querySerializer;
            _logger = logger;
        }

        public async Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var type = query.GetType();
            var data = _querySerializer.Serialize(query);

            using (var context = _dbContextFactory.Create())
            {
                await context.Queries.AddAsync(new Query
                {
                    Name = type.Name,
                    Type = type.FullName,
                    Data = data,
                    Id = query.Id,
                    Correlation = query.Correlation,
                    Timestamp = query.Timestamp,
                    Actor = query.Actor,
                });

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
