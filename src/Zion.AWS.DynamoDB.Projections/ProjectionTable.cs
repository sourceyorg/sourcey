using Amazon.DynamoDBv2.DataModel;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections
{
    public class ProjectionTable<TProjection>
        where TProjection : class, IProjection

    {
        [DynamoDBHashKey]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
