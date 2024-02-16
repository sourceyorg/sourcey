namespace Sourcey.Testing.Integration.Stubs;

using Microsoft.AspNetCore.Builder;

public sealed class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();
        await app.RunAsync();
    }
}
