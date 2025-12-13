using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using Newtonsoft.Json.Linq;
using Sourcey.Extensions;
using Sourcey.Keys;

namespace Sourcey.Newtonsoft.Json.Tests;

public class ResolversAndExtensionsTests
{
    private sealed class PrivateSetDto
    {
        public StreamId StreamId { get; private set; }
        public string Name { get; private set; }

        // non-public default ctor is allowed by deserializer settings
        private PrivateSetDto() { }
    }

    [Then]
    public void ImmutablePropertyResolver_allows_private_setters_to_be_deserialized()
    {
        var services = new ServiceCollection();
        services.AddSourcey().AddNewtonsoftJsonSerialization(b => b.WithEvents());
        using var provider = services.BuildServiceProvider();

        var serializer = provider.GetRequiredService<Sourcey.Events.Serialization.IEventSerializer>();
        var deserializer = provider.GetRequiredService<Sourcey.Events.Serialization.IEventDeserializer>();

        var json = serializer.Serialize(new { streamId = StreamId.From("s-1"), name = "N" });
        var obj = (PrivateSetDto)deserializer.Deserialize(json, typeof(PrivateSetDto));

        obj.StreamId.ToString().ShouldBe("s-1");
        obj.Name.ShouldBe("N");
    }

    // Note: PropertyInfoExtensions.HasSetter is internal; we validate its effect indirectly via
    // ImmutablePropertyCamelCasePropertyNamesContactResolver in the test above.
}

public class NullableConvertersWriteTests
{
    [Then]
    public void NullableConverters_Write_non_null_values()
    {
        var serializer = new JsonSerializer();

        // Actor?
        var wa = new Sourcey.Newtonsoft.Json.Converters.NullableActorJsonConverter();
        var aWriter = new JTokenWriter();
        wa.WriteJson(aWriter, Actor.From("a1"), serializer);
        aWriter.Token!.ToString().ShouldBe("a1");

        // EventId?
        var we = new Sourcey.Newtonsoft.Json.Converters.NullableEventIdJsonConverter();
        var eWriter = new JTokenWriter();
        we.WriteJson(eWriter, EventId.From("e1"), serializer);
        eWriter.Token!.ToString().ShouldBe("e1");

        // StreamId?
        var ws = new Sourcey.Newtonsoft.Json.Converters.NullableStreamIdJsonConverter();
        var sWriter = new JTokenWriter();
        ws.WriteJson(sWriter, StreamId.From("s1"), serializer);
        sWriter.Token!.ToString().ShouldBe("s1");

        // Causation?
        var wc = new Sourcey.Newtonsoft.Json.Converters.NullableCausationJsonConverter();
        var cWriter = new JTokenWriter();
        wc.WriteJson(cWriter, Causation.From("c1"), serializer);
        cWriter.Token!.ToString().ShouldBe("c1");

        // Correlation?
        var wk = new Sourcey.Newtonsoft.Json.Converters.NullableCorrelationJsonConverter();
        var kWriter = new JTokenWriter();
        wk.WriteJson(kWriter, Correlation.From("k1"), serializer);
        kWriter.Token!.ToString().ShouldBe("k1");
    }

    [Then]
    public void NullableConverters_Write_null_values()
    {
        var serializer = new JsonSerializer();

        void AssertNullToken(JToken? token)
        {
            token.ShouldNotBeNull();
            token!.Type.ShouldBe(JTokenType.Null);
        }

        var a = new Sourcey.Newtonsoft.Json.Converters.NullableActorJsonConverter();
        var aw = new JTokenWriter();
        a.WriteJson(aw, null, serializer);
        AssertNullToken(aw.Token);

        var e = new Sourcey.Newtonsoft.Json.Converters.NullableEventIdJsonConverter();
        var ew = new JTokenWriter();
        e.WriteJson(ew, null, serializer);
        AssertNullToken(ew.Token);

        var s = new Sourcey.Newtonsoft.Json.Converters.NullableStreamIdJsonConverter();
        var sw = new JTokenWriter();
        s.WriteJson(sw, null, serializer);
        AssertNullToken(sw.Token);

        var c = new Sourcey.Newtonsoft.Json.Converters.NullableCausationJsonConverter();
        var cw = new JTokenWriter();
        c.WriteJson(cw, null, serializer);
        AssertNullToken(cw.Token);

        var k = new Sourcey.Newtonsoft.Json.Converters.NullableCorrelationJsonConverter();
        var kw = new JTokenWriter();
        k.WriteJson(kw, null, serializer);
        AssertNullToken(kw.Token);
    }
}
