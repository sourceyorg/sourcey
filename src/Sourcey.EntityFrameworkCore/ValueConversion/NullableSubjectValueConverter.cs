using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.ValueConversion;

internal sealed class NullableSubjectValueConverter : ValueConverter<Subject?, string>
{
    public NullableSubjectValueConverter(ConverterMappingHints mappingHints = default)
        : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
    {
    }

    private static string ConvertTo(Subject? subject)
        => subject.HasValue ? (string)subject.Value : string.Empty;

    private static Subject? ConvertFrom(string subject)
        => string.IsNullOrWhiteSpace(subject) ? null : Subject.From(subject);
}
