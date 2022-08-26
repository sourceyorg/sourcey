using Xunit;
using Xunit.Sdk;

namespace Zion.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("Zion.Testing.Discoverers.PerformanceTraitDiscoverer", "Zion.Testing")]
    public class PerformanceAttribute : FactAttribute, ITraitAttribute { }
}
