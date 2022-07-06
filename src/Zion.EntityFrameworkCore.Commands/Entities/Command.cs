using Zion.Commands;
using Zion.Core.Keys;

namespace Zion.EntityFrameworkCore.Commands.Entities
{
    public class Command
    {
        public long SequenceNo { get; set; }
        public CommandId Id { get; set; }
        public Subject Subject { get; set; }
        public Correlation? Correlation { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public Actor Actor { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
