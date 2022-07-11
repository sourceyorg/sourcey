using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Xunit.Abstractions;
using Zion.AWS.DynamoDB.Projections.Extensions;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Core.Keys;
using Zion.Projections;
using Zion.Projections.Serialization;
using Zion.Testing.Extensions;

namespace Zion.AWS.DynamoDB.Projections.Tests.ProjectionWriter
{
    public abstract class ProjectionWriterSpecification : DynamoDBProjectionsSpecification<TestProjection, TestProjection>
    {
        protected readonly IProjectionTableInitializer _projectionTableInitializer;
        protected readonly IProjectionWriter<TestProjection> _projectionWriter;
        protected readonly ITestOutputHelper _testOutputHelper;
        protected readonly string _tableName = Subject.New();

        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            base.BuildServices(services);
            services.RemoveAll<DynamoDBConfiguration>();
            services.RegisterDynamoDBWriter<TestProjection>(o =>
            {
                o.Key = _configurationFixture.Configuration.GetValue<string>("AWS:DynamoDB:Key");
                o.Secret = _configurationFixture.Configuration.GetValue<string>("AWS:DynamoDB:Secret");
                o.ClientConfig = new()
                {
                    ServiceURL = _configurationFixture.Configuration.GetValue<string>("AWS:DynamoDB:Host")
                };
                o.TableOptions = (@override, table) =>
                {
                    table.TableName = _tableName;
                    table.KeySchema = new() { new KeySchemaElement(nameof(TestProjection.Something), KeyType.HASH) };
                    table.AttributeDefinitions = new() { new AttributeDefinition(nameof(TestProjection.Something), ScalarAttributeType.S) };
                    table.ProvisionedThroughput = new();
                    table.BillingMode = BillingMode.PAY_PER_REQUEST;
                };
            });

            services.AddSingleton<IProjectionSerializer, ProjectionSerializer>();
            services.AddSingleton<IProjectionDeserializer, ProjectionDeserializer>();

            return services;
        }

        public ProjectionWriterSpecification(DynamoDBProjectionsConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper) : base(configurationFixture, testOutputHelper)
        {
            _projectionTableInitializer = _serviceProvider.GetRequiredService<IProjectionTableInitializer>();
            _projectionWriter = _serviceProvider.GetRequiredService<IProjectionWriter<TestProjection>>();
            _testOutputHelper = testOutputHelper;
        }

        private class ProjectionSerializer : IProjectionSerializer
        {
            public string Serialize<T>(T data)
                => JsonConvert.SerializeObject(data);
        }

        private class ProjectionDeserializer : IProjectionDeserializer
        {
            public object Deserialize(string data, Type type)
                => JsonConvert.DeserializeObject(data, type);

            public T Deserialize<T>(string data)
                => JsonConvert.DeserializeObject<T>(data);
        }
    }
}
