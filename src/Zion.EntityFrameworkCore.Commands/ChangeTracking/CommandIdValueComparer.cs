using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zion.Commands;

namespace Zion.EntityFrameworkCore.Commands.ChangeTracking
{
    internal sealed class CommandIdValueComparer : ValueComparer<CommandId>
    {
        public CommandIdValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(CommandId left, CommandId right)
            => left.Equals(right);
        private static int HashCode(CommandId commandId)
            => commandId.GetHashCode();
        private static CommandId CreateSnapshot(CommandId commandId)
            => ConvertFrom(ConvertTo(commandId));
        private static string ConvertTo(CommandId commandId)
            => (string)commandId;
        private static CommandId ConvertFrom(string commandId)
            => CommandId.From(commandId);
    }
}
