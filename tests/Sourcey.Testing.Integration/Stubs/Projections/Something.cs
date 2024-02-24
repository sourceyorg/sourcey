using Sourcey.Projections;

namespace Sourcey.Testing.Integration.Stubs.Projections;

public class Something : IProjection
{
    public string Subject { get; set; }
    public int? Version { get; set; }
    public string Value { get; set; }
} 
