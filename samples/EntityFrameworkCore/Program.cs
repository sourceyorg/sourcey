using System.Reflection;
using EntityFrameworkCore.Aggregates;
using EntityFrameworkCore.Events;
using EntityFrameworkCore.Projections;
using EntityFrameworkCore.Projections.DbContexts;
using EntityFrameworkCore.Projections.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sourcey.Aggregates;
using Sourcey.Aggregates.Stores;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.Extensions;
using Sourcey.Projections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Action<DbContextOptionsBuilder> dbOptions = (o) => o.UseSqlServer(
    builder.Configuration.GetConnectionString("Projections"),
    b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

builder.Services.AddDbContextFactory<WriteableSomethingContext>(dbOptions);
builder.Services.AddPooledDbContextFactory<ReadonlySomethingContext>(dbOptions);
builder.Services.AddDbContextFactory<EventStoreDbContext>(o => o.UseSqlServer(
    builder.Configuration.GetConnectionString("EventStore"),
    b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
));

builder.Services.AddSourcey(sourceyBuilder =>
{
    sourceyBuilder.AddAggregate<SampleAggreagte, SampleState>();

    sourceyBuilder.AddEvents(e =>
    {
        e.RegisterEventCache<SomethingHappened>();
        e.WithEntityFrameworkCoreEventStore<EventStoreDbContext>(x =>
        {
            x.AddAggregate<SampleAggreagte, SampleState>();
            x.AddProjection<Something>();
        });
    });

    sourceyBuilder.AddProjection<Something>(x =>
    {
        x.WithManager<SomethingManager>();
        x.WithEntityFrameworkCoreWriter(e => e.WithContext<WriteableSomethingContext>());
        x.WithEntityFrameworkCoreReader(e => e.WithContext<ReadonlySomethingContext>());
        x.WithEntityFrameworkCoreStateManager(e => e.WithContext<WriteableSomethingContext>());
    });

    sourceyBuilder.AddSerialization(x =>
    {
        x.WithEvents();
        x.WithAggregates();
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
    var projections = await projectionReader.QueryAsync(cancellationToken);
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
    var projection = await projectionReader.ReadAsync(subject, cancellationToken);
    return projection;
})
.WithName("GetSample")
.WithTags("Sample")
.WithOpenApi();

await app.InitializeSourceyAsync();
await app.RunAsync();

record SampleRequest(string Something);
