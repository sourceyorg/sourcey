using InMemory.Aggregates;
using InMemory.Events;
using InMemory.Projections;
using InMemory.Projections.Managers;
using Microsoft.AspNetCore.Mvc;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.Extensions;
using Sourcey.Keys;
using Sourcey.Projections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSourcey(builder =>
{
    builder.AddAggregate<SampleAggreagte, SampleState>();

    builder.AddEvents(e =>
    {
        e.RegisterEventCache<SomethingHappened>();
        e.WithInMemoryStore(x =>
        {
            x.AddAggregate<SampleAggreagte, SampleState>();
            x.AddProjection<Something>();
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
        x.AddEventSerialization();
        x.AddAggregateSerialization();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/sample", async (
    [FromServices] IAggregateFactory aggregateFactory,
    [FromServices] IAggregateStore<SampleAggreagte, SampleState> aggregateStore,
    [FromBody] SampleRequest request,
    CancellationToken cancellationToken) =>
{
    var aggregate = aggregateFactory.Create<SampleAggreagte, SampleState>();
    aggregate.MakeSomethingHappen(request.Something);
    await aggregateStore.SaveAsync(aggregate, cancellationToken);

    return Results.Accepted(aggregate.Id);
})
.WithName("AddSample")
.WithTags("Sample")
.WithOpenApi();

app.MapGet("/sample", async (
    [FromServices] IProjectionReader<Something> projectionReader,
    CancellationToken cancellationToken) =>
{
    var projections = await projectionReader.ReadAllAsync(cancellationToken);
    return projections;
})
.WithName("GetSamples")
.WithTags("Sample")
.WithOpenApi();

app.MapGet("/sample/{subject}", async (
    [FromServices] IProjectionReader<Something> projectionReader,
    [FromRoute] string subject,
    CancellationToken cancellationToken) =>
{
    var projection = await projectionReader.ReadAsync(Subject.From(subject), cancellationToken);
    return projection;
})
.WithName("GetSample")
.WithTags("Sample")
.WithOpenApi();

await app.RunAsync();

record SampleRequest(string Something);
