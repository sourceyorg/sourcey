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
public class WhenConsistencyIsMatchedOnRead : EntityFrameworkIntegrationSpecification
{
    private readonly string _subject = Subject.New();
    private ValueTask<Something?> consistencyCheck;
    private IServiceScope _scope;

    public WhenConsistencyIsMatchedOnRead(
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
        consistencyCheck = projectionReader.ReadWithConsistencyAsync(_subject, s => s != null && s.Subject == _subject,
            5, TimeSpan.FromMilliseconds(5));
        return Task.CompletedTask;
    }

    protected override async Task When()
    {
        using var scope = _factory.Services.CreateScope();
        var aggregateFactory = scope.ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregateStore = scope.ServiceProvider.GetRequiredService<IAggregateStore<SampleAggreagte, SampleState>>();

        var aggregate = aggregateFactory.Create<SampleAggreagte, SampleState>();
        aggregate.MakeSomethingHappen(StreamId.From(_subject), "Something");
        await aggregateStore.SaveAsync(aggregate, default);
    }

    [Integration]
    public async Task ProjectionWithSubject_Should_BeInResult()
    {
        var result = await consistencyCheck;
        _scope.Dispose();
        result.ShouldNotBeNull().Subject.ShouldBe(_subject);
    }
}
