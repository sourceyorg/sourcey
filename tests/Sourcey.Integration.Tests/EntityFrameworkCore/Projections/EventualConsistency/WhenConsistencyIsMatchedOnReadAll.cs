using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.Keys;
using Sourcey.Projections;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Projections;
using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections.EventualConsistency;

[Collection(nameof(EntityFrameworkIntegrationCollection))]
public class WhenConsistencyIsMatchedOnReadAll : EntityFrameworkIntegrationSpecification
{
    private readonly Subject _subject = Subject.New();
    private  ValueTask<IQueryableProjection<Something>> consistencyCheck;
    private IServiceScope _scope;
    
    public WhenConsistencyIsMatchedOnReadAll(
        HostFixture hostFixture, EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper)
        : base(hostFixture, factory, testOutputHelper)
    {
    }

    protected override Task Given()
    {
        _scope = _factory.Services.CreateScope();
        var projectionReader = _scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();
        consistencyCheck = projectionReader.QueryAsync(q => new(q.Any(s => s.Subject == _subject.ToString())), 5, TimeSpan.FromMilliseconds(1));
        return Task.CompletedTask;
    }

    protected override async Task When()
    {
        using var scope = _factory.Services.CreateScope(); 
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();
        
        var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
        aggregate.MakeSomethingHappen(StreamId.From(_subject), "Something");
        await aggregateStore.SaveAsync(aggregate, default); 
    }

    [Integration]
    public async Task ProjectionWithSubject_Should_BeInResult()
    {
        var result = await consistencyCheck;
        _scope.Dispose();
        result.Where(s => s.Subject == _subject.ToString()).ShouldHaveSingleItem().Subject.ShouldBe(_subject);
    }
}
