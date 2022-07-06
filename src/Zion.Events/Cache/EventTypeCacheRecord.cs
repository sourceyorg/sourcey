using Zion.Core.Extensions;

namespace Zion.Events.Cache
{
    internal sealed record EventTypeCacheRecord
    {
        public EventTypeCacheRecord(Type type)
        {
            Key = type.FriendlyFullName();
            Type = type;
        }

        public string Key { get; }
        public Type Type { get; }
    }
}
