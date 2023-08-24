using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Redis;
using Xunit.Abstractions;
using Sourcey.Aggregates;
using Sourcey.Extensions;

namespace Sourcey.Redis.Aggregates.Tests.Snapshots
{
    public abstract class SnapshotSpecification<TAggregate, TAggregateState> : Specification
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        protected readonly RedisContainer _redisContainer = new RedisBuilder()
            .Build();
        
        protected override void BuildServices(IServiceCollection services)
        {
            services.AddSourcey()
                .AddJsonSerialization(o => o.AddAggregateSerialization())
                .AddAggregate<TAggregate, TAggregateState>(a => 
                a.WithRedisSnapshotStrategy(o 
                    => o.ConnectionString = _redisContainer.GetConnectionString()));

            base.BuildServices(services);
        }

        protected SnapshotSpecification(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        public override async Task InitializeAsync()
        {
            await _redisContainer.StartAsync();
            await base.InitializeAsync();
        }

        public override async Task DisposeAsync()
        {
            await _redisContainer.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
