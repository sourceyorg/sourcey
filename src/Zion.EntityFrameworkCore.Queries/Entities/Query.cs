﻿using Zion.Core.Keys;
using Zion.Queries;

namespace Zion.EntityFrameworkCore.Queries.Entities
{
    public class Query
    {
        public long SequenceNo { get; set; }
        public QueryId Id { get; set; }
        public Correlation? Correlation { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public Actor Actor { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
