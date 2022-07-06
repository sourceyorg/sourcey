using Amazon.DynamoDBv2;

namespace Zion.AWS.DynamoDB.Projections.Factories
{
    public interface IDynamoDBClientFactory
    {
        AmazonDynamoDBClient Create<TService>()
            where TService : class;
        AmazonDynamoDBClient Create<TService>(DynamoDBOptions options)
            where TService : class;
    }
}
