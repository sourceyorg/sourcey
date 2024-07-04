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
public class WhenConsistencyIsMatchedOnReadWithCustomReturn : EntityFrameworkIntegrationSpecification
{
    private readonly record struct SomethingProjection(string Value);
    
    private const string Value = "Something";
    
    private readonly string _subject = Subject.New();
    private ValueTask<SomethingProjection?> consistencyCheck;
    private IServiceScope _scope;

    public WhenConsistencyIsMatchedOnReadWithCustomReturn(
        ProjectionsDbFixture projectionsDbFixture,
        EventStoreDbFixture eventStoreDbFixture, EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper)
        : base(projectionsDbFixture, eventStoreDbFixture, factory, testOutputHelper)
    {
    }


    protected override Task Given()
    {
        _scope = _factory.Services.CreateScope();
        var projectionReader = _scope.ServiceProvider.GetRequiredService<IProjectionReader<Something>>();
        consistencyCheck = projectionReader.ReadAsync<SomethingProjection?>(
            subject: _subject,
            projection: s => new SomethingProjection(s.Value),
            consistencyCheck: s => s != null && s.Subject == _subject,
            retryCount: 5, 
            delay: TimeSpan.FromMilliseconds(5));
        return Task.CompletedTask;
    }

    protected override async Task When()
    {
        using var scope = _factory.Services.CreateScope();
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggregate, SampleState>>();

        var aggregate = aggregateFactory.Create<SampleAggregate, SampleState>();
        aggregate.MakeSomethingHappen(StreamId.From(_subject), Value);
        await aggregateStore.SaveAsync(aggregate, default);
    }

    [Integration]
    public async Task ProjectionWithSubject_Should_BeInResult()
    {
        var result = await consistencyCheck;
        _scope.Dispose();
        result.ShouldNotBeNull().Value.ShouldBe(Value);
    }
}
