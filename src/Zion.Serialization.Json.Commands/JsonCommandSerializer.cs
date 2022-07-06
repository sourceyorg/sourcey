using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Zion.Commands.Serialization;

namespace Zion.Serialization.Json.Commands
{
    internal class JsonCommandSerializer : ICommandSerializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonCommandSerializer(IEnumerable<JsonConverter> jsonConverters)
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
                Converters = jsonConverters?.ToList() ?? new List<JsonConverter>()
            };
        }

        public string Serialize<T>(T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.SerializeObject(data, _serializerSettings);
        }
    }
}
