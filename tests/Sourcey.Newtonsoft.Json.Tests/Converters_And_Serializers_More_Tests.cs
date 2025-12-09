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
}

public class SerializerDeserializerMoreTests
{
    private sealed record AggregateDto(StreamId StreamId, string Name, Causation? Causation, Correlation? Correlation);

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
    public void AggregateSerializer_throws_on_null()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithAggregates());
        using var provider = services.BuildServiceProvider();

        var sut = provider.GetRequiredService<Sourcey.Aggregates.Serialization.IAggregateSerializer>();
        Should.Throw<ArgumentNullException>(new Action(() => sut.Serialize<object>(null!)));
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
}
