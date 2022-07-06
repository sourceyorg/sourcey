using Microsoft.Extensions.DependencyInjection;

namespace Zion.Encryption.Builder
{
    public interface IZionEncryptionBuilder
    {
        IServiceCollection Services { get; }
    }
}
