using Microsoft.Extensions.DependencyInjection;

namespace Zion.Serialization.Json.Builder
{
    public interface IZionJsonSerializationBuilder
    {
        IServiceCollection Services { get; }
    }
}
