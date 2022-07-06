using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace Zion.AWS.DynamoDB.Projections.Factories
{
    public record DynamoDBOptions
    {
        public string Key { get; set; }
        public string Secret { get; set; }
        public string? Region { get; set; }
        public bool AutoCreateTables { get; set; } = true;
        public AmazonDynamoDBConfig? ClientConfig { get; set; }
        public DynamoDBContextConfig? ContextConfig { get; set; }
        public Action<string?, CreateTableRequest>? TableOptions { get; set; }
    }
}
