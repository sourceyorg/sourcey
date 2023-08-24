using Xunit;
using Xunit.Sdk;

namespace Sourcey.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("Sourcey.Testing.Discoverers.PerformanceTraitDiscoverer", "Sourcey.Testing")]
    public class PerformanceAttribute : FactAttribute, ITraitAttribute { }
}
