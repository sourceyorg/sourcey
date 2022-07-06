using System;
using Xunit;
using Xunit.Sdk;

namespace Zion.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("Zion.Testing.Discoverers.IntegrationTraitDiscoverer", "Zion.Testing")]
    public class IntegrationAttribute : FactAttribute, ITraitAttribute { }
}
