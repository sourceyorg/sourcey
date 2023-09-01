using Sourcey.Keys;
using Sourcey.Projections;

namespace InMemory.Projections;

public class Something : IProjection
{
    public string Subject { get; set; }
    public int? Version { get; set; }
    public string Value { get; set; }
} 