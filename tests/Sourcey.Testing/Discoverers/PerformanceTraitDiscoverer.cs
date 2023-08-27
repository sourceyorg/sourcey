using Xunit.Abstractions;
using Xunit.Sdk;

namespace Sourcey.Testing.Discoverers;

public class PerformanceTraitDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", "Performance");
    }
}
