using Zion.Core.Keys;

namespace Zion.Commands
{
    public abstract record Command : ICommand
    {
        public CommandId Id { get; }
        public Subject Subject { get; }
        public Correlation? Correlation { get; }
        public int? Version { get; }
        public Actor Actor { get; }
        public DateTimeOffset Timestamp { get; }

        public Command(Subject subject, Correlation? correlationId, int? version, Actor actor)
        {
            Id = CommandId.New();
            Subject = subject;
            Correlation = correlationId;
            Version = version;
            Actor = actor;
            Timestamp = DateTimeOffset.UtcNow;
        }

        public Command(Subject subject, Correlation? correlationId, Actor actor) : this(subject, correlationId, null, actor ) { }
    }
}
