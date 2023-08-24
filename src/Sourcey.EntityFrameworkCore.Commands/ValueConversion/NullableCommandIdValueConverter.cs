using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Commands;

namespace Sourcey.EntityFrameworkCore.Commands.ValueConversion
{
    internal sealed class NullableCommandIdValueConverter : ValueConverter<CommandId?, string>
    {
        public NullableCommandIdValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(CommandId? commandId)
            => commandId.HasValue ? (string)commandId.Value : string.Empty;

        private static CommandId? ConvertFrom(string commandId)
            => string.IsNullOrWhiteSpace(commandId) ? null : CommandId.From(commandId);
    }
}
