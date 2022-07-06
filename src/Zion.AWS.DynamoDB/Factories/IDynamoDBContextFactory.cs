using Amazon.DynamoDBv2.DataModel;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections.Factories
{
    public interface IDynamoDBContextFactory
    {
        DynamoDBContext Create<TProjection>()
            where TProjection : class, IProjection;
    }
}
