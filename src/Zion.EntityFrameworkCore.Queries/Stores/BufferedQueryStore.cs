﻿using Microsoft.Extensions.DependencyInjection;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Queries.Entities;
using Zion.EntityFrameworkCore.Queries.Factories;
using Zion.Queries;
using Zion.Queries.Serialization;
using Zion.Queries.Stores;

namespace Zion.EntityFrameworkCore.Queries.Stores
{
    internal sealed class BufferedQueryStore : Zion.Queries.Stores.BufferedQueryStore
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQuerySerializer _querySerializer;

        public BufferedQueryStore(IServiceScopeFactory serviceScopeFactory,
            IQuerySerializer querySerializer)
        {
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (querySerializer == null)
                throw new ArgumentNullException(nameof(querySerializer));

            _serviceScopeFactory = serviceScopeFactory;
            _querySerializer = querySerializer;
        }

        protected override async Task ConsumeAsync(IQuery<object> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var type = query.GetType();
            var data = _querySerializer.Serialize(query);
            
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IQueryStoreDbContextFactory>();

            using var context = dbContextFactory.Create();
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