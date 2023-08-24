namespace Sourcey.Serialization.Json.Events
{
    public sealed class EventNotificationPayload
    {
        public string StreamId { get; set; }
        public string? Correlation { get; set; }
        public string? Causation { get; set; }
        public string Payload { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Actor { get; set; }
    }
}
