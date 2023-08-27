using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Core.Keys;

namespace Sourcey.EntityFrameworkCore.ValueConversion;

internal sealed class NullableActorValueConverter : ValueConverter<Actor?, string>
{
    public NullableActorValueConverter(ConverterMappingHints mappingHints = default)
        : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
    {
    }

    private static string ConvertTo(Actor? actor)
        => actor.HasValue ? (string)actor.Value : string.Empty;

    private static Actor? ConvertFrom(string actor)
        => string.IsNullOrWhiteSpace(actor) ? null : Actor.From(actor);
}
