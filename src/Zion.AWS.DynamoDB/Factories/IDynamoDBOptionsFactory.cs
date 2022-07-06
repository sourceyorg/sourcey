using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections.Factories
{
    public interface IDynamoDBOptionsFactory
    {
        DynamoDBOptions Create<TService>()
            where TService : class;
    }
}
