using Amazon.DynamoDBv2;

namespace Zion.AWS.DynamoDB.Projections.Factories
{
    internal sealed class DynamoDBClientFactory : IDynamoDBClientFactory
    {
        private readonly IDynamoDBOptionsFactory _optionsFactory;

        public DynamoDBClientFactory(IDynamoDBOptionsFactory optionsFactory)
        {
            if (optionsFactory is null)
                throw new ArgumentNullException(nameof(optionsFactory));

            _optionsFactory = optionsFactory;
        }

        public AmazonDynamoDBClient Create<TService>()
            where TService : class
        {
            var options = _optionsFactory.Create<TService>();
            return Create<TService>(options);
        }

        public AmazonDynamoDBClient Create<TService>(DynamoDBOptions options)
            where TService : class
            => BuildClient(options);

        private static AmazonDynamoDBClient BuildClient(DynamoDBOptions options) => options switch
        {
            _ when string.IsNullOrWhiteSpace(options.Region) && options.ClientConfig is null
                => new AmazonDynamoDBClient(options.Key, options.Secret),

            _ when string.IsNullOrWhiteSpace(options.Region) && options.ClientConfig is not null
                => new AmazonDynamoDBClient(options.Key, options.Secret, options.ClientConfig),

            _ when !string.IsNullOrWhiteSpace(options.Region) && options.ClientConfig is not null
                => new AmazonDynamoDBClient(options.Key, options.Secret, options.Region, options.ClientConfig),

            _ => new AmazonDynamoDBClient(options.Key, options.Secret, options.Region)
        };
    }
}
