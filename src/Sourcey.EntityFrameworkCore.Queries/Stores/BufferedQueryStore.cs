using Microsoft.Extensions.DependencyInjection;
using Sourcey.Core.Extensions;
using Sourcey.EntityFrameworkCore.Queries.DbContexts;
using Sourcey.EntityFrameworkCore.Queries.Entities;
using Sourcey.Queries;
using Sourcey.Queries.Serialization;

namespace Sourcey.EntityFrameworkCore.Queries.Stores
{
    internal sealed class BufferedQueryStore<TQueryStoreDbContext> : Sourcey.Queries.Stores.BufferedQueryStore<TQueryStoreDbContext>
        where TQueryStoreDbContext : QueryStoreDbContext
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
            using var context = scope.ServiceProvider.GetRequiredService<TQueryStoreDbContext>();

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
