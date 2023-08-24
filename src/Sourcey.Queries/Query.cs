using System;
using Sourcey.Core.Keys;

namespace Sourcey.Queries
{
    public abstract record Query<TQueryResult> : IQuery<TQueryResult>
    {
        public QueryId Id { get; }
        public DateTimeOffset Timestamp { get; }
        public Correlation? Correlation { get; }
        public Actor Actor { get; }

        public Query(Correlation? correlation, Actor actor)
        {
            Id = QueryId.New();
            Timestamp = DateTimeOffset.UtcNow;
            Correlation = correlation;
            Actor = actor;
        }
    }
}
