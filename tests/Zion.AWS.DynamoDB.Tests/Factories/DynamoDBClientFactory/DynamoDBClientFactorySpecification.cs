using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Zion.AWS.DynamoDB.Extensions;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Testing.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Zion.AWS.DynamoDB.Tests.Factories.DynamoDBClientFactory
{
    public abstract class DynamoDBClientFactorySpecification : SpecificationWithConfiguration<DynamoDBConfigurationFixture>
    {
        protected IDynamoDBClientFactory Factory => _serviceProvider.GetRequiredService<IDynamoDBClientFactory>();

        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            services.TryAddDynamoDBFactories();

            return base.BuildServices(services);
        }
        protected DynamoDBClientFactorySpecification(DynamoDBConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper) 
            : base(configurationFixture, testOutputHelper)
        {
        }
    }

    public abstract class DynamoDBClientFactorySpecification<TResult> : SpecificationWithConfiguration<DynamoDBConfigurationFixture, TResult>
    {
        protected IDynamoDBClientFactory Factory => _serviceProvider.GetRequiredService<IDynamoDBClientFactory>();

        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            services.TryAddDynamoDBFactories();

            return base.BuildServices(services);
        }
        protected DynamoDBClientFactorySpecification(DynamoDBConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
        }
    }
}
