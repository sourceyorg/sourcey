﻿using System.Collections.Concurrent;
using System.Reflection;

namespace Zion.Queries.Cache
{
    internal sealed class QueryTypeCache : IQueryTypeCache
    {
        public readonly ConcurrentDictionary<string, Type> _lookup;

        public QueryTypeCache(IEnumerable<QueryTypeCacheRecord> cacheRecords)
        {
            if (cacheRecords?.Any() == true)
                _lookup = new ConcurrentDictionary<string, Type>(cacheRecords.DistinctBy(cr => cr.Key).ToDictionary(cr => cr.Key, cr => cr.Type));
            else 
                _lookup = new();
        }

        public bool TryGet(string name, out Type? type)
        {
            if (_lookup.TryGetValue(name, out type))
                return true;

            var potentialMatches = new List<Type>();

            foreach (var key in _lookup.Keys)
            {
                var part = key.Split('.').Last().Split('+').Last();

                if (part.Equals(name, StringComparison.OrdinalIgnoreCase))
                    potentialMatches.Add(_lookup[key]);
            }

            if (potentialMatches.Count < 1)
                return false;

            if (potentialMatches.Count > 1)
            {
                var typeNames = string.Join(", ", potentialMatches.Select(t => $"'{t.FullName}'"));
                throw new AmbiguousMatchException($"Multiple types are registered with the same name, but different namespaces. The types are: {typeNames}");
            }

            type = potentialMatches[0];

            return true;
        }
    }
}
