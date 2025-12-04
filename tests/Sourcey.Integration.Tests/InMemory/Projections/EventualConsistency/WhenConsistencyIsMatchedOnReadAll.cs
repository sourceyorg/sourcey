using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.Keys;
using Sourcey.Projections;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Projections;
using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.InMemory.Projections.EventualConsistency;

[Collection(nameof(InMemoryIntegrationCollection))]
public class WhenConsistencyIsMatchedOnReadAll : InMemorySpecification
{
    private readonly Subject _subject = Subject.New();
    private  Task<IQueryableProjection<Something>> consistencyCheck;
    private IServiceScope _scope;
    
    public WhenConsistencyIsMatchedOnReadAll(ITestOutputHelper testOutputHelper,
        InMemoryWebApplicationFactory factory)
        : base(factory, testOutputHelper)
    {
    }

    protected override async Task Given()
    {
        _scope = _factory.Services.CreateAsyncScope();  
        var projectionReader = _scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();
        consistencyCheck = projectionReader.QueryAsync(q => new(q.Any(s => s.Subject == _subject)), 5, TimeSpan.FromMilliseconds(1)).AsTask();
    }

    protected override async Task When()
    {
        await using var scope = _factory.Services.CreateAsyncScope(); 
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();
        
        var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
        aggregate.MakeSomethingHappen(StreamId.From(_subject), "Something");
        await aggregateStore.SaveAsync(aggregate, default).ConfigureAwait(false); 
    }

    [Integration]
    public async Task ProjectionWithSubject_Should_BeInResult()
    {
        var result = await consistencyCheck;
        _scope.Dispose();
        result.Where(s => s.Subject == _subject).ShouldHaveSingleItem().Subject.ShouldBe(_subject);
    }
}
