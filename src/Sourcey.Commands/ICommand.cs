using Sourcey.Core.Keys;

namespace Sourcey.Commands
{
    public interface ICommand
    {
        CommandId Id { get; }
        Subject Subject { get; }
        Correlation? Correlation { get; }
        DateTimeOffset Timestamp { get; }
        int? Version { get; }
        Actor Actor { get; }
    }
}
