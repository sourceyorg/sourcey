using Sourcey.Core.Keys;
using Sourcey.Events;
using Sourcey.Events.Streams;

namespace Sourcey.EntityFrameworkCore.Events.Entities;

public class Event
{
    public long SequenceNo { get; set; }
    public EventId Id { get; set; }
    public StreamId StreamId { get; set; }
    public Correlation? Correlation { get; set; }
    public Causation? Causation { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Data { get; set; }
    public Actor Actor { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset? ScheduledPublication { get; set; }
    public int? Version { get; set; }
}
