using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Core.Extensions;
using Zion.Projections;
using Zion.Projections.Serialization;

namespace Zion.AWS.DynamoDB.Projections
{
    internal sealed class ProjectionWriter<TProjection> : IProjectionWriter<TProjection>
        where TProjection : class, IProjection
    {
        private readonly IDynamoDBContextFactory _dynamoDBContextFactory;
        private readonly IDynamoDBOptionsFactory _dBOptionsFactory;
        private readonly IProjectionSerializer _projectionSerializer;
        private readonly IProjectionDeserializer _projectionDeserializer;
        private readonly IProjectionTableInitializer _projectionInitializer;
        private readonly ILogger<ProjectionWriter<TProjection>> _logger;

        public ProjectionWriter(IDynamoDBContextFactory dynamoDBContextFactory,
            IDynamoDBOptionsFactory dBOptionsFactory,
            IProjectionSerializer projectionSerializer,
            IProjectionDeserializer projectionDeserializer,
            IProjectionTableInitializer projectionInitializer,
            ILogger<ProjectionWriter<TProjection>> logger)
        {
            if (dynamoDBContextFactory is null)
                throw new ArgumentNullException(nameof(dynamoDBContextFactory));
            if (dBOptionsFactory is null)
                throw new ArgumentNullException(nameof(dBOptionsFactory));
            if (projectionSerializer is null)
                throw new ArgumentNullException(nameof(projectionSerializer));
            if (projectionDeserializer is null)
                throw new ArgumentNullException(nameof(projectionDeserializer));
            if (projectionInitializer is null)
                throw new ArgumentNullException(nameof(projectionInitializer));
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            _dynamoDBContextFactory = dynamoDBContextFactory;
            _dBOptionsFactory = dBOptionsFactory;
            _projectionSerializer = projectionSerializer;
            _projectionDeserializer = projectionDeserializer;
            _projectionInitializer = projectionInitializer;
            _logger = logger;
        }


        public async Task<TProjection> AddAsync(string subject, Func<TProjection> add, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(AddAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var entity = add();

            using var context = _dynamoDBContextFactory.Create<TProjection>();

            var options = _dBOptionsFactory.Create<TProjection>();

            if(options.TableOptions is not null)
            {
                await context.SaveAsync(entity, BuildDynamoDBOperationConfig(options.TableOptions), cancellationToken);

                return entity;
            }

            var projection = new ProjectionTable<TProjection>
            {
                Key = subject,
                Value = _projectionSerializer.Serialize(entity)
            };

            await context.SaveAsync(projection, BuildDynamoDBOperationConfig(options.TableOptions), cancellationToken);

            return entity;
        }

        public async Task RemoveAsync(string subject, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(AddAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _dynamoDBContextFactory.Create<TProjection>();
            var options = _dBOptionsFactory.Create<TProjection>();

            await context.DeleteAsync<ProjectionTable<TProjection>>(subject, BuildDynamoDBOperationConfig(options.TableOptions), cancellationToken);
        }

        public async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(AddAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await _projectionInitializer.ResetTableAsync<TProjection>();
        }

        public async Task<TProjection> UpdateAsync(string subject, Func<TProjection, TProjection> update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _dynamoDBContextFactory.Create<TProjection>();

            TProjection? entity = null;
            var options = _dBOptionsFactory.Create<TProjection>();
            var config = BuildDynamoDBOperationConfig(options.TableOptions);

            if (options.TableOptions is null)
            {
                var projection = await context.LoadAsync<ProjectionTable<TProjection>>(subject, config, cancellationToken);

                if (projection is null)
                    return null;

                entity = _projectionDeserializer.Deserialize<TProjection>(projection.Value);
                update(entity);
                projection.Value = _projectionSerializer.Serialize(entity);
                await context.SaveAsync(projection, config, cancellationToken);
            }
            else
            {
                entity = await context.LoadAsync<TProjection>(subject, config, cancellationToken);
                update(entity);
                await context.SaveAsync(entity, config, cancellationToken);
            }

            return entity;
        }

        public async Task<TProjection> UpdateAsync(string subject, Action<TProjection> update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _dynamoDBContextFactory.Create<TProjection>();

            TProjection? entity = null;
            var options = _dBOptionsFactory.Create<TProjection>();
            var config = BuildDynamoDBOperationConfig(options.TableOptions);

            if (options.TableOptions is null)
            {
                var projection = await context.LoadAsync<ProjectionTable<TProjection>>(subject, config, cancellationToken);

                if (projection is null)
                    return null;

                entity = _projectionDeserializer.Deserialize<TProjection>(projection.Value);
                update(entity);
                projection.Value = _projectionSerializer.Serialize(entity);
                await context.SaveAsync(projection, config, cancellationToken);
            }
            else
            {
                entity = await context.LoadAsync<TProjection>(subject, config, cancellationToken);
                update(entity);
                await context.SaveAsync(entity, config, cancellationToken);
            }

            return entity;
        }

        private DynamoDBOperationConfig BuildDynamoDBOperationConfig(Action<string?, CreateTableRequest>? options = null)
        {
            var request = new CreateTableRequest()
            {
                TableName = typeof(TProjection).FriendlyName()
            };

            if (options is not null)
                options(null, request);

            return new DynamoDBOperationConfig
            {
                OverrideTableName = request.TableName
            };
        }
    }
}
