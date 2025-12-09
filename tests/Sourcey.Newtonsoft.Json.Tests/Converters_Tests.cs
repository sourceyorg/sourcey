using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Sourcey.Extensions;
using Sourcey.Keys;
using Sourcey.Newtonsoft.Json.Converters;
using Sourcey.Events.Serialization;

namespace Sourcey.Newtonsoft.Json.Tests;

public class ConvertersTests
{
    [Then]
    public void ActorJsonConverter_roundtrips_value()
    {
        var converter = new ActorJsonConverter();
        var serializer = new JsonSerializer();

        var writer = new JTokenWriter();
        converter.WriteJson(writer, Actor.From("actor-123"), serializer);

        var token = writer.Token;
        token.ShouldNotBeNull();
        token!.Type.ShouldBe(JTokenType.String);
        token.ToString().ShouldBe("actor-123");

        var reader = new JTokenReader(token);
        var result = converter.ReadJson(reader, typeof(Actor), default, false, serializer);
        result.ToString().ShouldBe("actor-123");
    }

    [Then]
    public void EventIdJsonConverter_roundtrips_value()
    {
        var converter = new EventIdJsonConverter();
        var serializer = new JsonSerializer();

        var writer = new JTokenWriter();
        converter.WriteJson(writer, EventId.From("evt-1"), serializer);

        var token = writer.Token!;
        token.Type.ShouldBe(JTokenType.String);

        var reader = new JTokenReader(token);
        var result = converter.ReadJson(reader, typeof(EventId), default, false, serializer);
        result.ToString().ShouldBe("evt-1");
    }

    [Then]
    public void StreamIdJsonConverter_roundtrips_value()
    {
        var converter = new StreamIdJsonConverter();
        var serializer = new JsonSerializer();

        var writer = new JTokenWriter();
        converter.WriteJson(writer, StreamId.From("stream-xyz"), serializer);

        var token = writer.Token!;
        token.Type.ShouldBe(JTokenType.String);

        var reader = new JTokenReader(token);
        var result = converter.ReadJson(reader, typeof(StreamId), default, false, serializer);
        result.ToString().ShouldBe("stream-xyz");
    }

    [Then]
    public void NullableActorJsonConverter_reads_null_as_null()
    {
        var converter = new NullableActorJsonConverter();
        var serializer = new JsonSerializer();

        var token = JValue.CreateNull();
        var reader = new JTokenReader(token);

        var result = converter.ReadJson(reader, typeof(Actor?), null, false, serializer);
        result.HasValue.ShouldBeFalse();
    }

    [Then]
    public void NullableEventIdJsonConverter_reads_null_as_null()
    {
        var converter = new NullableEventIdJsonConverter();
        var serializer = new JsonSerializer();

        var token = JValue.CreateNull();
        var reader = new JTokenReader(token);

        var result = converter.ReadJson(reader, typeof(EventId?), null, false, serializer);
        result.HasValue.ShouldBeFalse();
    }

    [Then]
    public void NullableStreamIdJsonConverter_reads_null_as_null()
    {
        var converter = new NullableStreamIdJsonConverter();
        var serializer = new JsonSerializer();

        var token = JValue.CreateNull();
        var reader = new JTokenReader(token);

        var result = converter.ReadJson(reader, typeof(StreamId?), null, false, serializer);
        result.HasValue.ShouldBeFalse();
    }
}

public class SerializerTests
{
    private static JsonConverter[] AllConverters() => new JsonConverter[]
    {
        new ActorJsonConverter(),
        new EventIdJsonConverter(),
        new StreamIdJsonConverter(),
        new NullableActorJsonConverter(),
        new NullableEventIdJsonConverter(),
        new NullableStreamIdJsonConverter()
    };

    private sealed record SampleDto(Actor Actor, EventId EventId, StreamId StreamId, string Name);

    [Then]
    public void EventSerializer_throws_on_null()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents());
        using var provider = services.BuildServiceProvider();

        var sut = provider.GetRequiredService<IEventSerializer>();
        Should.Throw<ArgumentNullException>(new Action(() => sut.Serialize<object>(null!)));
    }

    [Then]
    public void EventSerializer_and_Deserializer_roundtrip_with_converters_and_camel_case()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents());
        using var provider = services.BuildServiceProvider();

        var serializer = provider.GetRequiredService<IEventSerializer>();
        var deserializer = provider.GetRequiredService<IEventDeserializer>();

        var dto = new SampleDto(Actor.From("actor-a"), EventId.From("evt-2"), StreamId.From("stream-1"), "Hello");

        var json = serializer.Serialize(dto);
        json.ShouldContain("actor"); // camelCase property names
        json.ShouldContain("actor-a");
        json.ShouldContain("evt-2");
        json.ShouldContain("stream-1");

        var back = deserializer.Deserialize<SampleDto>(json);
        back.Actor.ToString().ShouldBe("actor-a");
        back.EventId.ToString().ShouldBe("evt-2");
        back.StreamId.ToString().ShouldBe("stream-1");
        back.Name.ShouldBe("Hello");
    }
}

public class DiRegistrationTests
{
    [Then]
    public void AddNewtonsoftJsonSerialization_registers_converters_and_serializers()
    {
        var services = new ServiceCollection();
        services.AddSourcey()
            .AddNewtonsoftJsonSerialization(b => b.WithEvents().WithAggregates());

        using var provider = services.BuildServiceProvider();

        // Converters are registered as IEnumerable<JsonConverter>
        var converters = provider.GetRequiredService<IEnumerable<JsonConverter>>().ToArray();
        converters.ShouldContain(c => c is ActorJsonConverter);
        converters.ShouldContain(c => c is EventIdJsonConverter);
        converters.ShouldContain(c => c is StreamIdJsonConverter);

        // Serializers/deserializers registered via WithEvents/WithAggregates
        provider.GetRequiredService<Sourcey.Events.Serialization.IEventSerializer>().ShouldNotBeNull();
        provider.GetRequiredService<Sourcey.Events.Serialization.IEventDeserializer>().ShouldNotBeNull();
        provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateSerializer>().ShouldNotBeNull();
        provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateDeserializer>().ShouldNotBeNull();
    }
}
