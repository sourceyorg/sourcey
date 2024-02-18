using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.InMemory;

public abstract class InMemorySpecification : Specification
{
    protected readonly InMemoryWebApplicationFactory _factory;

    protected InMemorySpecification(
        InMemoryWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper ) : base(testOutputHelper)
    {
        _factory = factory;
    }
}
