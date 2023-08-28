using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.ValueConversion;

internal sealed class SubjectValueConverter : ValueConverter<Subject, string>
{
    public SubjectValueConverter(ConverterMappingHints mappingHints = default)
        : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
    {
    }

    private static string ConvertTo(Subject actor)
        => (string)actor;

    private static Subject ConvertFrom(string actor)
        => Subject.From(actor);
}
