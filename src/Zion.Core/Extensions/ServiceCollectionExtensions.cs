using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Core.Builder;
using Zion.Core.Exceptions;

namespace Zion.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IZionBuilder AddZion(this IServiceCollection services)
        {
            services.TryAddScoped<IExceptionStream, ExceptionStream>();
            return new ZionBuilder(services);
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
}
