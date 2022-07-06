using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections.Factories
{
    internal sealed class DynamoDBContextFactory : IDynamoDBContextFactory
    {
        private readonly IDynamoDBClientFactory _dynamoDBClientFactory;
        private readonly IDynamoDBOptionsFactory _optionsFactory;

        public DynamoDBContextFactory(IDynamoDBClientFactory dynamoDBClientFactory,
            IDynamoDBOptionsFactory optionsFactory)
        {
            if (dynamoDBClientFactory is null)
                throw new ArgumentNullException(nameof(dynamoDBClientFactory));
            if (optionsFactory is null)
                throw new ArgumentNullException(nameof(optionsFactory));

            _dynamoDBClientFactory = dynamoDBClientFactory;
            _optionsFactory = optionsFactory;
        }

        public DynamoDBContext Create<TProjection>()
            where TProjection : class, IProjection
        {
            var options = _optionsFactory.Create<TProjection>();
            var client = _dynamoDBClientFactory.Create<TProjection>(options);
            var context = BuildContext(options.ContextConfig, client);

            return context;
        }

        private static DynamoDBContext BuildContext(DynamoDBContextConfig? config, AmazonDynamoDBClient client) => config switch
        {
            _ when config is null => new DynamoDBContext(client),
            _ => new DynamoDBContext(client, config)
        };
    }
}
