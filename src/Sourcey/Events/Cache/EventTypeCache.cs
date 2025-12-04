using System.Collections.Concurrent;
using System.Reflection;

namespace Sourcey.Events.Cache;

internal sealed class EventTypeCache(IEnumerable<EventTypeCacheRecord> cacheRecords) : IEventTypeCache
{
    public readonly ConcurrentDictionary<string, Type> _lookup =
        cacheRecords?.ToArray() is { Length: > 0 } cacheRecordsArray
            ? new ConcurrentDictionary<string, Type>(cacheRecordsArray.DistinctBy(cr => cr.Key)
                .ToDictionary(cr => cr.Key, cr => cr.Type))
            : [];


    public bool TryGet(string name, out Type? type)
    {
        if (_lookup.TryGetValue(name, out type))
            return true;

        var matchingKeys = MatchingKeys(name)
            .Take(2)
            .ToArray();

        switch (matchingKeys.Length)
        {
            case 0:
                return false;
            case 1:
                type = _lookup[matchingKeys[0]];
                return true;
            default:
                throw new AmbiguousMatchException(
                    $"Multiple types are registered with the same name, but different namespaces. The types are: {string.Join(", ", MatchingKeys(name).Select(t => $"'{_lookup[t].FullName}'"))}");
        }
    }

    public bool ContainsKey(string name)
    {
        if (_lookup.ContainsKey(name))
            return true;

        var potentialMatches = MatchingKeys(name)
            .Select(k => _lookup[k].FullName ?? k)
            .Take(2)
            .ToArray();

        return potentialMatches.Length switch
        {
            0 => false,
            1 => true,
            _ => throw new AmbiguousMatchException(
                $"Multiple types are registered with the same name, but different namespaces. The types are: {string.Join(", ", potentialMatches)}")
        };
    }

    private IEnumerable<string> MatchingKeys(string name)
    {
        foreach (var key in _lookup.Keys)
        {
            var part = key.Split('.').Last().Split('+').Last();

            if (part.Equals(name, StringComparison.OrdinalIgnoreCase))
                yield return key;
        }
    }
}
