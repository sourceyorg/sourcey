using Sourcey.Integration.Host.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddPostgres(DistributedApplicationKeys.EventStore);
builder.AddPostgres(DistributedApplicationKeys.Projections);

builder.Build().Run();
