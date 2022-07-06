using Microsoft.Extensions.DependencyInjection;

namespace Zion.Encryption.Builder
{
    internal readonly struct ZionEncryptionBuilder : IZionEncryptionBuilder
    {
        public readonly IServiceCollection Services { get; }

        public ZionEncryptionBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
