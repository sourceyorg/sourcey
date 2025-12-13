using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Sourcey.Extensions;
using Sourcey.Keys;
using Sourcey.Newtonsoft.Json.Converters;

namespace Sourcey.Newtonsoft.Json.Tests;

public class MoreConvertersTests
{
    [Then]
    public void CausationJsonConverter_roundtrips_value()
    {
        var converter = new CausationJsonConverter();
        var serializer = new JsonSerializer();

        var writer = new JTokenWriter();
        converter.WriteJson(writer, Causation.From("caus-1"), serializer);

        var token = writer.Token!;
        token.Type.ShouldBe(JTokenType.String);
        token.ToString().ShouldBe("caus-1");

        var reader = new JTokenReader(token);
        var result = converter.ReadJson(reader, typeof(Causation), default, false, serializer);
        result.ToString().ShouldBe("caus-1");
    }

    [Then]
    public void CorrelationJsonConverter_roundtrips_value()
    {
        var converter = new CorrelationJsonConverter();
        var serializer = new JsonSerializer();

        var writer = new JTokenWriter();
        converter.WriteJson(writer, Correlation.From("corr-1"), serializer);

        var token = writer.Token!;
        token.Type.ShouldBe(JTokenType.String);
        token.ToString().ShouldBe("corr-1");

        var reader = new JTokenReader(token);
        var result = converter.ReadJson(reader, typeof(Correlation), default, false, serializer);
        result.ToString().ShouldBe("corr-1");
    }

    [Then]
    public void NullableCausationJsonConverter_reads_null_as_null()
    {
        var converter = new NullableCausationJsonConverter();
        var serializer = new JsonSerializer();

        var token = JValue.CreateNull();
        var reader = new JTokenReader(token);

        var result = converter.ReadJson(reader, typeof(Causation?), null, false, serializer);
        result.HasValue.ShouldBeFalse();
    }

    [Then]
    public void NullableCorrelationJsonConverter_reads_null_as_null()
    {
        var converter = new NullableCorrelationJsonConverter();
        var serializer = new JsonSerializer();

        var token = JValue.CreateNull();
        var reader = new JTokenReader(token);

        var result = converter.ReadJson(reader, typeof(Correlation?), null, false, serializer);
        result.HasValue.ShouldBeFalse();
    }

    [Then]
    public void NonNullableConverters_coerce_non_string_tokens_via_serializer()
    {
        var serializer = new JsonSerializer();

        // Integer token should be coerced to string by serializer and then wrapped by converters
        var intToken = new JValue(123);

        var actor = new ActorJsonConverter().ReadJson(new JTokenReader(intToken), typeof(Actor), default, false, serializer);
        actor.ToString().ShouldBe("123");

        var evt = new EventIdJsonConverter().ReadJson(new JTokenReader(intToken), typeof(EventId), default, false, serializer);
        evt.ToString().ShouldBe("123");

        var stream = new StreamIdJsonConverter().ReadJson(new JTokenReader(intToken), typeof(StreamId), default, false, serializer);
        stream.ToString().ShouldBe("123");

        var caus = new CausationJsonConverter().ReadJson(new JTokenReader(intToken), typeof(Causation), default, false, serializer);
        caus.ToString().ShouldBe("123");

        var corr = new CorrelationJsonConverter().ReadJson(new JTokenReader(intToken), typeof(Correlation), default, false, serializer);
        corr.ToString().ShouldBe("123");
    }
}

public class SerializerDeserializerMoreTests
{
    private sealed record AggregateDto(StreamId StreamId, string Name, Causation? Causation, Correlation? Correlation);
    private sealed record EventDto(StreamId StreamId, string Name);

    [Then]
    public void EventDeserializer_throws_on_null()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents());
        using var provider = services.BuildServiceProvider();

        var sut = provider.GetRequiredService<Sourcey.Events.Serialization.IEventDeserializer>();
        Should.Throw<ArgumentNullException>(new Action(() => sut.Deserialize<object>(null!)));
    }

    [Then]
    public void EventDeserializer_non_generic_throws_on_null()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents());
        using var provider = services.BuildServiceProvider();

        var sut = provider.GetRequiredService<Sourcey.Events.Serialization.IEventDeserializer>();
        Should.Throw<ArgumentNullException>(new Action(() => sut.Deserialize(null!, typeof(object))));
    }

    [Then]
    public void AggregateSerializer_throws_on_null()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithAggregates());
        using var provider = services.BuildServiceProvider();

        var sut = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateSerializer>();
        Should.Throw<ArgumentNullException>(new Action(() => sut.Serialize<object>(null!)));
    }

    [Then]
    public void AggregateDeserializer_non_generic_throws_on_null()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithAggregates());
        using var provider = services.BuildServiceProvider();

        var sut = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateDeserializer>();
        Should.Throw<ArgumentNullException>(new Action(() => sut.Deserialize(null!, typeof(object))));
    }

    [Then]
    public void AggregateSerializer_and_Deserializer_roundtrip_with_converters_and_camel_case()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithAggregates());
        using var provider = services.BuildServiceProvider();

        var serializer = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateSerializer>();
        var deserializer = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateDeserializer>();

        var dto = new AggregateDto(StreamId.From("stream-aggr"), "Hello", Causation.From("caus-x"), Correlation.From("corr-y"));

        var json = serializer.Serialize(dto);
        json.ShouldContain("streamId"); // camelCase property names
        json.ShouldContain("stream-aggr");
        json.ShouldContain("caus-x");
        json.ShouldContain("corr-y");

        var back = deserializer.Deserialize<AggregateDto>(json);
        back.StreamId.ToString().ShouldBe("stream-aggr");
        back.Name.ShouldBe("Hello");
        back.Causation!.Value.ToString().ShouldBe("caus-x");
        back.Correlation!.Value.ToString().ShouldBe("corr-y");
    }

    [Then]
    public void EventDeserializer_non_generic_roundtrip()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents());
        using var provider = services.BuildServiceProvider();

        var serializer = provider.GetRequiredService<Sourcey.Events.Serialization.IEventSerializer>();
        var deserializer = provider.GetRequiredService<Sourcey.Events.Serialization.IEventDeserializer>();

        var dto = new EventDto(StreamId.From("evt-stream"), "Name");
        var json = serializer.Serialize(dto);

        var obj = deserializer.Deserialize(json, typeof(EventDto));
        obj.ShouldBeOfType<EventDto>().ShouldSatisfyAllConditions(
            e => e.StreamId.ToString().ShouldBe("evt-stream"),
            e => e.Name.ShouldBe("Name")
        );
    }

    [Then]
    public void AggregateDeserializer_non_generic_roundtrip()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithAggregates());
        using var provider = services.BuildServiceProvider();

        var serializer = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateSerializer>();
        var deserializer = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateDeserializer>();

        var dto = new AggregateDto(StreamId.From("agg-stream"), "A", Causation.From("c"), Correlation.From("k"));
        var json = serializer.Serialize(dto);

        var obj = deserializer.Deserialize(json, typeof(AggregateDto));
        obj.ShouldBeOfType<AggregateDto>().ShouldSatisfyAllConditions(
            a => a.StreamId.ToString().ShouldBe("agg-stream"),
            a => a.Name.ShouldBe("A"),
            a => a.Causation!.Value.ToString().ShouldBe("c"),
            a => a.Correlation!.Value.ToString().ShouldBe("k")
        );
    }

    [Then]
    public void DI_registers_additional_converters_for_causation_and_correlation()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents().WithAggregates());
        using var provider = services.BuildServiceProvider();

        var converters = provider.GetRequiredService<IEnumerable<JsonConverter>>().ToArray();
        converters.ShouldContain(c => c is CausationJsonConverter);
        converters.ShouldContain(c => c is CorrelationJsonConverter);
        converters.ShouldContain(c => c is NullableCausationJsonConverter);
        converters.ShouldContain(c => c is NullableCorrelationJsonConverter);
    }

    private sealed record UnknownFriendlyDto(StreamId StreamId, string Name);

    [Then]
    public void Deserializers_ignore_unknown_fields_by_default()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents().WithAggregates());
        using var provider = services.BuildServiceProvider();

        var eventSerializer = provider.GetRequiredService<Sourcey.Events.Serialization.IEventSerializer>();
        var eventDeserializer = provider.GetRequiredService<Sourcey.Events.Serialization.IEventDeserializer>();
        var aggregateSerializer = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateSerializer>();
        var aggregateDeserializer = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateDeserializer>();

        var dto = new UnknownFriendlyDto(StreamId.From("s-1"), "N");

        // Inject an unknown property into the payload
        var evtJson = JObject.Parse(eventSerializer.Serialize(dto));
        evtJson["unknownProp"] = "xyz";
        var evtBack = eventDeserializer.Deserialize<UnknownFriendlyDto>(evtJson.ToString());
        evtBack.StreamId.ToString().ShouldBe("s-1");
        evtBack.Name.ShouldBe("N");

        var aggJson = JObject.Parse(aggregateSerializer.Serialize(dto));
        aggJson["unknownProp"] = "xyz";
        var aggBack = aggregateDeserializer.Deserialize<UnknownFriendlyDto>(aggJson.ToString());
        aggBack.StreamId.ToString().ShouldBe("s-1");
        aggBack.Name.ShouldBe("N");
    }
}
