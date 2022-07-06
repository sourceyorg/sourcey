using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Core.Extensions;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections
{
    internal sealed class ProjectionInitializer : IProjectionTableInitializer
    {
        private readonly IDynamoDBClientFactory _clientFactory;
        private readonly IDynamoDBOptionsFactory _optionsFactory;
        private readonly ILogger<ProjectionInitializer> _logger;

        public ProjectionInitializer(IDynamoDBClientFactory clientFactory,
            IDynamoDBOptionsFactory optionsFactory,
            ILogger<ProjectionInitializer> logger)
        {
            if (clientFactory is null)
                throw new ArgumentNullException(nameof(clientFactory));
            if (optionsFactory is null)
                throw new ArgumentNullException(nameof(optionsFactory));
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            _clientFactory = clientFactory;
            _optionsFactory = optionsFactory;
            _logger = logger;
        }

        public async Task CreateTableAsync<TProjection>(string? tableOverride = null, CancellationToken cancellationToken = default)
            where TProjection : class, IProjection
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionInitializer)}.{nameof(CreateTableAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }
            var options = _optionsFactory.Create<TProjection>();

            var request = new CreateTableRequest();
            var action = options.TableOptions ?? DefaultOptions<TProjection>();

            action(tableOverride, request);

            using var client = _clientFactory.Create<TProjection>(options);
            try
            {
                await client.CreateTableAsync(request, cancellationToken);
            }
            catch(ResourceInUseException)
            {
                _logger.LogInformation($"{nameof(ProjectionInitializer)}.{nameof(CreateTableAsync)}: table for {request.TableName} already exists");
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteTableAsync<TProjection>(string? tableOverride = null, CancellationToken cancellationToken = default)
            where TProjection : class, IProjection
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionInitializer)}.{nameof(DeleteTableAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var client = _clientFactory.Create<TProjection>();
            await client.DeleteTableAsync(tableOverride ?? typeof(TProjection).FriendlyName(), cancellationToken);
        }

        public async Task ResetTableAsync<TProjection>(string? tableOverride = null, CancellationToken cancellationToken = default)
            where TProjection : class, IProjection
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionInitializer)}.{nameof(ResetTableAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            await DeleteTableAsync<TProjection>(tableOverride, cancellationToken);
            await CreateTableAsync<TProjection>(tableOverride, cancellationToken);
        }

        internal static Action<string?, CreateTableRequest> DefaultOptions<TProjection>()
            where TProjection : class, IProjection 
            => (tableOverride, options) =>
            {
                options.TableName = tableOverride ?? typeof(TProjection).FriendlyName();
                options.KeySchema = new() { new KeySchemaElement(nameof(ProjectionTable<TProjection>.Key), KeyType.HASH) };
                options.AttributeDefinitions = new() { new AttributeDefinition(nameof(ProjectionTable<TProjection>.Key), ScalarAttributeType.S) };
                options.ProvisionedThroughput = new();
                options.BillingMode = BillingMode.PAY_PER_REQUEST;
            };
    }
}
