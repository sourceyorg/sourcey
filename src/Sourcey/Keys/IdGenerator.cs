using Sourcey.Extensions;

namespace Sourcey.Keys;

public static class IdGenerator
{
    public static string New()
        => Guid.NewGuid()
            .ToSequentialGuid()
            .ToString();
}
