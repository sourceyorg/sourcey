using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Core.Extensions;
using Zion.Core.Keys;
using Zion.Projections;
using Zion.Projections.Serialization;

namespace Zion.AWS.DynamoDB.Projections
{
    internal sealed class ProjectionReader<TProjection> : IProjectionReader<TProjection>
        where TProjection : class, IProjection
    {
        private readonly IDynamoDBContextFactory _dynamoDBContextFactory;
        private readonly IProjectionDeserializer _projectionDeserializer;
        private readonly IDynamoDBOptionsFactory _dBOptionsFactory;
        private readonly ILogger<ProjectionReader<TProjection>> _logger;

        public ProjectionReader(IDynamoDBContextFactory dynamoDBContextFactory,
            IDynamoDBOptionsFactory dBOptionsFactory,
            IProjectionDeserializer projectionDeserializer, 
            ILogger<ProjectionReader<TProjection>> logger)
        {
            if(dynamoDBContextFactory is null)
                throw new ArgumentNullException(nameof(dynamoDBContextFactory));
            if (dBOptionsFactory is null)
                throw new ArgumentNullException(nameof(dBOptionsFactory));
            if (projectionDeserializer is null)
                throw new ArgumentNullException(nameof(projectionDeserializer));
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            _dynamoDBContextFactory = dynamoDBContextFactory;
            _dBOptionsFactory = dBOptionsFactory;
            _projectionDeserializer = projectionDeserializer;
            _logger = logger;
        }

        public async Task<TProjection?> RetrieveAsync(Subject subject, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(RetrieveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _dynamoDBContextFactory.Create<TProjection>();
            TProjection? entity = null;
            var options = _dBOptionsFactory.Create<TProjection>();
            var config = BuildDynamoDBOperationConfig(options.TableOptions);

            if (options.TableOptions is null)
            {
                var projection = await context.LoadAsync<ProjectionTable<TProjection>>(subject.ToString(), config, cancellationToken);

                if (projection is null)
                    return null;

                entity = _projectionDeserializer.Deserialize<TProjection>(projection.Value);
            }
            else
            {
                entity = await context.LoadAsync<TProjection>(subject.ToString(), config, cancellationToken);
            }

            return entity;
        }

        public Task<IEnumerable<TProjection>> RetrieveAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
