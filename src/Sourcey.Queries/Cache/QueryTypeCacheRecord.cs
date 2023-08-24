using Sourcey.Core.Extensions;

namespace Sourcey.Queries.Cache
{
    internal sealed record QueryTypeCacheRecord
    {
        public QueryTypeCacheRecord(Type type)
        {
            Key = type.FriendlyName();
            Type = type;
        }

        public string Key { get; }
        public Type Type { get; }
    }
}
