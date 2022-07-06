using Microsoft.Extensions.Logging;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Core.Extensions;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections
{
    internal class ProjectionStateManager<TProjection> : IProjectionStateManager<TProjection>
        where TProjection : class, IProjection
    {
        private readonly IDynamoDBContextFactory _dynamoDBClientFactory;
        private readonly ILogger<ProjectionStateManager<TProjection>> _logger;
        private readonly string _key;

        public ProjectionStateManager(IDynamoDBContextFactory dynamoDBClientFactory,
            ILogger<ProjectionStateManager<TProjection>> logger)
        {
            if (dynamoDBClientFactory == null)
                throw new ArgumentNullException(nameof(dynamoDBClientFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dynamoDBClientFactory = dynamoDBClientFactory;
            _logger = logger;
            _key = $"ProjectionState_{typeof(TProjection).FriendlyName()}";
        }

        public async Task RemoveAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(RemoveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _dynamoDBClientFactory.Create<TProjection>();

            await context.DeleteAsync(_key, cancellationToken);
        }

        public async Task<IProjectionState> RetrieveAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _dynamoDBClientFactory.Create<TProjection>();
            return await context.LoadAsync<ProjectionState>(_key, cancellationToken);
        }

        public async Task<IProjectionState> UpdateAsync(Action<IProjectionState> update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _dynamoDBClientFactory.Create<TProjection>();

            var entity = await context.LoadAsync<ProjectionState>(_key, cancellationToken) 
                ?? new ProjectionState 
                {
                    Key = _key,
                    CreatedDate = DateTimeOffset.UtcNow,
                    Position = 1
                };

            update(entity);

            await context.SaveAsync(entity, cancellationToken);

            return entity;
        }
    }
}
