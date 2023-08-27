using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Sourcey.Testing.Abstractions;

namespace Sourcey.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddXunitLogging(this IServiceCollection services, ITestOutputHelper testOutputHelper)
    {
        services.AddSingleton(testOutputHelper);
        services.AddTransient(typeof(ILogger<>), typeof(XunitLogger<>));
        return services;
    }
}
