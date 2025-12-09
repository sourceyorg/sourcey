using Newtonsoft.Json;
using Shouldly;
using Sourcey.Keys;
using Sourcey.Newtonsoft.Json.Converters;

namespace Sourcey.Newtonsoft.Json.Tests.Keys;

public class WhenSerializingStreamId
{
    private sealed record Wrapper(StreamId Id);

    [Then]
    public void Then_it_round_trips_via_converter()
    {
        var id = StreamId.New();
        var settings = new JsonSerializerSettings
        {
            Converters = { new StreamIdJsonConverter() }
        };

        var json = JsonConvert.SerializeObject(new Wrapper(id), settings);
        var roundTripped = JsonConvert.DeserializeObject<Wrapper>(json, settings)!;

        roundTripped.Id.ShouldBe(id);
    }

    [Then]
    public void Then_it_treats_null_as_unknown_when_reading()
    {
        var settings = new JsonSerializerSettings
        {
            Converters = { new StreamIdJsonConverter() }
        };

        var json = JsonConvert.SerializeObject(new { Id = (string?)null });
        var value = JsonConvert.DeserializeObject<Wrapper>(json, settings)!;

        value.Id.ShouldBe(StreamId.Unknown);
    }

    [Then]
    public void Then_it_reads_plain_string_value_into_streamid()
    {
        var id = StreamId.New();
        var settings = new JsonSerializerSettings
        {
            Converters = { new StreamIdJsonConverter() }
        };

        var json = JsonConvert.SerializeObject(id.ToString());
        var read = JsonConvert.DeserializeObject<StreamId>(json, settings);
        read.ShouldBe(id);
    }
}
