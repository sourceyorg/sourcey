﻿using Microsoft.Extensions.DependencyInjection;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Queries.DbContexts;
using Zion.EntityFrameworkCore.Queries.Entities;
using Zion.Queries;
using Zion.Queries.Serialization;
using Zion.Queries.Stores;

namespace Zion.EntityFrameworkCore.Queries.Stores
{
    internal sealed class QueryStore : IQueryStore
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQuerySerializer _querySerializer;

        public QueryStore(IServiceScopeFactory serviceScopeFactory,
            IQuerySerializer querySerializer)
        {
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (querySerializer == null)
                throw new ArgumentNullException(nameof(querySerializer));

            _serviceScopeFactory = serviceScopeFactory;
            _querySerializer = querySerializer;
        }

        public async Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var type = query.GetType();
            var data = _querySerializer.Serialize(query);
            
            using var scope = _serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<QueryStoreDbContext>();

            await context.Queries.AddAsync(new Query
            {
                Name = type.Name,
                Type = type.FriendlyFullName(),
                Data = data,
                Id = query.Id,
                Correlation = query.Correlation,
                Timestamp = query.Timestamp,
                Actor = query.Actor,
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
