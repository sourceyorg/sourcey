using Newtonsoft.Json;
using Zion.Encryption;

namespace Zion.Serialization.Json.Converters
{
    public sealed class SecretJsonConverter : JsonConverter<Secret>
    {
        public override void WriteJson(JsonWriter writer, Secret value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override Secret ReadJson(JsonReader reader, Type objectType, Secret existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<byte[]>(reader);
            return Secret.From(value ?? Array.Empty<byte>());
        }
    }
}
