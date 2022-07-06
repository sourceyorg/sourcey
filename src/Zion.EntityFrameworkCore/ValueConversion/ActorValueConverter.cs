using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Core.Keys;

namespace Zion.EntityFrameworkCore.ValueConversion
{
    internal sealed class ActorValueConverter : ValueConverter<Actor, string>
    {
        public ActorValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(Actor actor)
            => (string)actor;

        private static Actor ConvertFrom(string actor)
            => Actor.From(actor);
    }
}
