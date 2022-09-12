using System.Text;
using Newtonsoft.Json;
using Zion.Encryption;

namespace Zion.Serialization.Json.Converters
{
    public sealed class SecretJsonConverter : JsonConverter<Secret>
    {
        public override void WriteJson(JsonWriter writer, Secret value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, Encoding.UTF8.GetString(value));
        }

        public override Secret ReadJson(JsonReader reader, Type objectType, Secret existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            return Secret.From(Encoding.UTF8.GetBytes(value ?? string.Empty) ?? Array.Empty<byte>());
        }
    }
}
