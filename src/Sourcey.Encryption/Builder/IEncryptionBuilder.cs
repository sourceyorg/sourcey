using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Encryption.Builder
{
    public interface IEncryptionBuilder
    {
        IServiceCollection Services { get; }
    }
}
