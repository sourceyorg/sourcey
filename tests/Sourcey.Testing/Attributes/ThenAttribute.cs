using Xunit;
using Xunit.Sdk;

namespace Sourcey.Testing.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
[TraitDiscoverer("Sourcey.Testing.Discoverers.ThenTraitDiscoverer", "Sourcey.Testing")]
public class ThenAttribute : FactAttribute, ITraitAttribute { }
