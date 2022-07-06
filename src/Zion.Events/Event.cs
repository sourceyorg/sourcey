namespace Zion.Events
{
    public abstract record Event : IEvent
    {
        public EventId Id { get; protected init; }
        public string Subject { get; protected init; }
        public DateTimeOffset Timestamp { get; protected init; }
        public int Version { get; protected init; }

        public Event(string subject, int version)
        {
            Id = EventId.New();
            Subject = subject;
            Timestamp = DateTimeOffset.UtcNow;
            Version = version;
        }
    }
}
