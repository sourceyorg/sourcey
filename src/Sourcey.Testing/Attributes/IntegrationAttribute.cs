using System;
using Xunit;
using Xunit.Sdk;

namespace Sourcey.Testing.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("Sourcey.Testing.Discoverers.IntegrationTraitDiscoverer", "Sourcey.Testing")]
    public class IntegrationAttribute : FactAttribute, ITraitAttribute { }
}
