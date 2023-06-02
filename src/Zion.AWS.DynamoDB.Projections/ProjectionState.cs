using Amazon.DynamoDBv2.DataModel;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections
{
    [DynamoDBTable(nameof(ProjectionState))]
    internal class ProjectionState : IProjectionState
    {
        [DynamoDBHashKey]
        public string Key { get; set; }
        public long Position { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string? Error { get; set; }
        public string? ErrorStackTrace { get; set; }
    }
}
