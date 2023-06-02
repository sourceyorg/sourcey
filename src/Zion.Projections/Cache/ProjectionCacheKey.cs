using Zion.Core.Keys;
using Zion.Events;

namespace Zion.Projections.Cache
{
    public struct ProjectionCacheKey
    {
        private string _key;

        public ProjectionCacheKey(string key)
        {
            _key = key;
        }

        public ProjectionCacheKey(string key, Actor actor, Subject subject, IEnumerable<IEvent> events)
        {
            _key = $"{key}:{actor}:{subject}:{string.Join(":", events.Select(e => e.Id.ToString()))}";
        }

        public Actor? Actor => _key.Split(":").FirstOrDefault() is { } actor ? Zion.Core.Keys.Actor.From(actor) : null;
        public Subject? Subject => _key.Split(":").Skip(1).FirstOrDefault() is { } subject ? Zion.Core.Keys.Subject.From(subject) : null;
        public IEnumerable<EventId> EventIds => _key.Split(":").Skip(2).Select(k => EventId.From(k));

        public override string ToString() => _key;
    }
}
