﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Builder;
using Sourcey.Exceptions;

namespace Sourcey.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static ISourceyBuilder AddSourcey(this IServiceCollection services)
    {
        services.TryAddScoped<IExceptionStream, ExceptionStream>();
        return new SourceyBuilder(services);
    }

    public static IServiceCollection AddSourcey(this IServiceCollection services, Action<ISourceyBuilder> options)
    {
        services.TryAddScoped<IExceptionStream, ExceptionStream>();
        options(new SourceyBuilder(services));

        return services;
    }

    public static IEnumerable<T> GetRequiredServices<T>(this IServiceProvider source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var services = source.GetRequiredService<IEnumerable<T>>();

        if (!services.Any())
            throw new InvalidOperationException($"No services could be found for '{typeof(T).FriendlyName()}'");

        return services;
    }
}
