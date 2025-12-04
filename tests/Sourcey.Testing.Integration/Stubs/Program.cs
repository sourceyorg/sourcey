using Sourcey.Extensions;
using Sourcey.Testing.Integration.Stubs.Aggregates;

namespace Sourcey.Testing.Integration.Stubs;

using Microsoft.AspNetCore.Builder;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSourcey(s => s.AddAggregate<SampleAggregate, SampleState>());

        var app = builder.Build();
        await app.InitializeSourceyAsync();
        await app.RunAsync();
    }
}
