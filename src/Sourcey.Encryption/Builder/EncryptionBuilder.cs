using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Encryption.Builder
{
    internal readonly struct EncryptionBuilder : IEncryptionBuilder
    {
        public readonly IServiceCollection Services { get; }

        public EncryptionBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
