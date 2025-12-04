using Microsoft.Extensions.DependencyInjection;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Builder;
using Xunit.Abstractions;

namespace Sourcey.Tests.Aggregates;

public abstract class AggregateSpecification<TAggregate, TAggregateState> : Specification<TAggregate>
    where TAggregate : Aggregate<TAggregateState>
    where TAggregateState : IAggregateState, new()
{
    private TAggregate _aggregate = default!;

    protected AggregateSpecification(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected abstract Task SetupAsync(TAggregate aggregate);

    protected virtual void Configure(IAggregateBuilder<TAggregate, TAggregateState> aggregateBuilder) {}

    protected override async Task When()
    {
        var aggregateFactory = ServiceProvider.GetRequiredService<IAggregateFactory>();
        var aggregate = aggregateFactory.Create<TAggregate, TAggregateState>();

        await SetupAsync(aggregate).ConfigureAwait(false);

        _aggregate = aggregate;
    }

    protected override Task<TAggregate> Given()
    {
        return Task.FromResult(_aggregate);
    }

    protected override void BuildServices(IServiceCollection services)
    {
        services.AddSourcey(s => {
            s.AddAggregate<TAggregate, TAggregateState>(Configure);
        });

        base.BuildServices(services);
    }
}