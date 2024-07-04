using Microsoft.AspNetCore.Hosting;
using Sourcey.Extensions;
using Sourcey.Testing.Integration.Abstractions;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Events;
using Sourcey.Testing.Integration.Stubs.Projections;
using Sourcey.Testing.Integration.Stubs.Projections.Managers;

namespace Sourcey.Integration.Tests.InMemory;

public class InMemoryWebApplicationFactory : SourceyWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSourcey(builder =>
            {
                builder.AddAggregate<SampleAggregate, SampleState>();
            
                builder.AddEvents(e =>
                {
                    e.RegisterEventCache<SomethingHappened>();
                    e.WithInMemoryStore(x =>
                    {
                        x.AddAggregate<SampleAggregate, SampleState>();
                        x.AddProjection<Something>(p=> p.WithInterval(1));
                    });
                });
            
                builder.AddProjection<Something>(x =>
                {
                    x.WithManager<SomethingManager>();
                    x.WithInMemoryWriter();
                    x.WithInMemoryStateManager();
                });
            
                builder.AddSerialization(x =>
                {
                    x.WithEvents();
                    x.WithAggregates();
                });
            });  
        });
        
        base.ConfigureWebHost(builder);
    }
}
