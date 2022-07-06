using Newtonsoft.Json;
using Zion.Encryption;

namespace Zion.Serialization.Json.Converters
{
    public sealed class NullableSecretJsonConverter : JsonConverter<Secret?>
    {
        public override void WriteJson(JsonWriter writer, Secret? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override Secret? ReadJson(JsonReader reader, Type objectType, Secret? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<byte[]>(reader);

            if (value == null)
                return null;

            return Secret.From(value);
        }
    }
}
