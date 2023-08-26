using Newtonsoft.Json;
using Sourcey.Events.Serialization;
using Sourcey.Serialization.Json.Resolvers;

namespace Sourcey.Serialization.Json.Events;

internal sealed class EventDeserializer : IEventDeserializer
{
    private readonly JsonSerializerSettings _serializerSettings;

    public EventDeserializer(IEnumerable<JsonConverter> jsonConverters)
    {
        _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new ImmutablePropertyCamelCasePropertyNamesContactResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = jsonConverters?.ToList() ?? new List<JsonConverter>()
        };
    }

    public object Deserialize(string data, Type type)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return JsonConvert.DeserializeObject(data, type, _serializerSettings);
    }
    public T Deserialize<T>(string data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return JsonConvert.DeserializeObject<T>(data, _serializerSettings);
    }
}
