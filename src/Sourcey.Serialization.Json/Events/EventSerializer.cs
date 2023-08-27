using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sourcey.Events.Serialization;

namespace Sourcey.Serialization.Json.Events;

internal class EventSerializer : IEventSerializer
{
    private readonly JsonSerializerSettings _serializerSettings;

    public EventSerializer(IEnumerable<JsonConverter> jsonConverters)
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
