using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zion.Commands;

namespace Zion.EntityFrameworkCore.Commands.ChangeTracking
{
    internal sealed class NullableCommandIdValueComparer : ValueComparer<CommandId?>
    {
        public NullableCommandIdValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(CommandId? left, CommandId? right)
        {
            if (left == null || right == null)
                return false;

            return left.Value.Equals(right.Value);
        }

        private static int HashCode(CommandId? commandId)
        {
            if (commandId == null)
                return 0;

            if (commandId is IEquatable<CommandId>)
                return commandId.GetHashCode();

            return ConvertTo(commandId)?.GetHashCode() ?? 0;
        }

        private static CommandId? CreateSnapshot(CommandId? commandId)
        {
            return ConvertFrom(ConvertTo(commandId));
        }

        private static string? ConvertTo(CommandId? commandId)
        {
            if (commandId == null)
                return null;

            return (string)commandId;
        }

        private static CommandId? ConvertFrom(string? commandId)
        {
            if (string.IsNullOrWhiteSpace(commandId))
                return null;

            return CommandId.From(commandId);
        }
    }
}
