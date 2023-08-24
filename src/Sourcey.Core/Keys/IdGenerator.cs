using Sourcey.Core.Extensions;

namespace Sourcey.Core.Keys
{
    public static class IdGenerator
    {
        public static string New()
            => Guid.NewGuid()
                .ToSequentialGuid()
                .ToString();
    }
}
