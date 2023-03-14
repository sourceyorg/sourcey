using Zion.Core.Extensions;

namespace Zion.Core.Keys
{
    public static class ZionIdGenerator
    {
        public static string New()
            => Guid.NewGuid()
                .ToSequentialGuid()
                .ToString();
    }
}
