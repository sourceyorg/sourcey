using Sourcey.Extensions;

namespace Sourcey.Keys;

public static class IdGenerator
{
    internal const string Unknown = "Unknown";

    public static string New()
        => Guid.NewGuid()
            .ToSequentialGuid()
            .ToString();
}
