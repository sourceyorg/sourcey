using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Commands;

namespace Zion.EntityFrameworkCore.Commands.ValueConversion
{
    internal sealed class CommandIdValueConverter : ValueConverter<CommandId, string>
    {
        public CommandIdValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(CommandId commandId)
            => (string)commandId;

        private static CommandId ConvertFrom(string commandId)
            => CommandId.From(commandId);
    }
}
