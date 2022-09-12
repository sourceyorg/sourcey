using System.Text;
using Newtonsoft.Json;
using Zion.Encryption;

namespace Zion.Serialization.Json.Converters
{
    public sealed class NullableSecretJsonConverter : JsonConverter<Secret?>
    {
        public override void WriteJson(JsonWriter writer, Secret? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.HasValue ? Encoding.UTF8.GetString(value.Value) : string.Empty);
        }

        public override Secret? ReadJson(JsonReader reader, Type objectType, Secret? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);

            if (string.IsNullOrWhiteSpace(value))
                return null;

            return Secret.From(Encoding.UTF8.GetBytes(value));
        }
    }
}
