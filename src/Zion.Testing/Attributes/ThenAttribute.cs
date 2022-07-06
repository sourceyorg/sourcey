using System;
using Xunit;
using Xunit.Sdk;

namespace Zion.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("Zion.Testing.Discoverers.ThenTraitDiscoverer", "Zion.Testing")]
    public class ThenAttribute : FactAttribute, ITraitAttribute { }
}
