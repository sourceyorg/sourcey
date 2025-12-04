using Sourcey.Integration.Host.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddPostgres(DistributedApplicationKeys.EventStore);
builder.AddPostgres(DistributedApplicationKeys.Projections);

await builder.Build().RunAsync().ConfigureAwait(false);
