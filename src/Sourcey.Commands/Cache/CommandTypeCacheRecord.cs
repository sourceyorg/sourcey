using Sourcey.Core.Extensions;

namespace Sourcey.Commands.Cache
{
    public sealed record CommandTypeCacheRecord
    {
        public CommandTypeCacheRecord(Type type)
        {
            Key = type.FriendlyName();
            Type = type;
        }

        public string Key { get; }
        public Type Type { get; }
    }
}
