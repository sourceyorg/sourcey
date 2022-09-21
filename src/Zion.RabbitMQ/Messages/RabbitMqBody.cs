using MessagePack;

namespace Zion.RabbitMQ.Messages
{
    [MessagePackObject]
    public sealed class RabbitMqBody 
    {
        [Key(0)]
        public string StreamId { get; set; }
        [Key(1)]
        public string? Correlation { get; set;}
        [Key(2)]
        public string? Causation { get; set; }
        [Key(3)]
        public string Payload { get; set; }
        [Key(4)]
        public DateTimeOffset Timestamp { get; set; }
        [Key(5)]
        public string Actor { get; set; }
    }
}
