using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Core.Extensions;
using Zion.Core.Initialization;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections.Initializers
{
    internal sealed class ProjectionInitializer<TProjection> : IZionInitializer
        where TProjection : class, IProjection
    {
        private readonly IDynamoDBClientFactory _clientFactory;
        private readonly IDynamoDBOptionsFactory _optionsFactory;
        private readonly IProjectionTableInitializer _projectionInitializer;
        private readonly ILogger<ProjectionInitializer<TProjection>> _logger;

        public ProjectionInitializer(IDynamoDBClientFactory clientFactory,
            IDynamoDBOptionsFactory optionsFactory,
            IProjectionTableInitializer projectionInitializer,
            ILogger<ProjectionInitializer<TProjection>> logger)
        {
            if(clientFactory is null)
                throw new ArgumentNullException(nameof(clientFactory));
            if (optionsFactory is null)
                throw new ArgumentNullException(nameof(optionsFactory));
            if (projectionInitializer is null)
                throw new ArgumentNullException(nameof(projectionInitializer));
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            _clientFactory = clientFactory;
            _optionsFactory = optionsFactory;
            _projectionInitializer = projectionInitializer;
            _logger = logger;
        }

        public bool ParallelEnabled => true;

        public async Task InitializeAsync(IHost host)
        {
            var options = _optionsFactory.Create<TProjection>();

            if(options.AutoCreateTables)
                await Task.WhenAll(_projectionInitializer.CreateTableAsync<TProjection>(), CreateProjectionStateTableAsync());
        }

        private async Task CreateProjectionStateTableAsync()
        {
            var request = new CreateTableRequest
            {
                TableName = nameof(ProjectionState),
                KeySchema = new() { new KeySchemaElement(nameof(ProjectionState.Key), KeyType.HASH) },
                AttributeDefinitions = new() { new AttributeDefinition(nameof(ProjectionState.Key), ScalarAttributeType.N) },
                ProvisionedThroughput = new(),
                BillingMode = BillingMode.PAY_PER_REQUEST
            };

            using var client = _clientFactory.Create<TProjection>();
            try
            {
                await client.CreateTableAsync(request);
            }
            catch (ResourceInUseException)
            {
                _logger.LogInformation($"{nameof(ProjectionInitializer<TProjection>)}.{nameof(CreateProjectionStateTableAsync)}: table for {typeof(TProjection).FriendlyName()}: {request.TableName} already exists");
            }
            catch
            {
                throw;
            }
        }
    }
}
