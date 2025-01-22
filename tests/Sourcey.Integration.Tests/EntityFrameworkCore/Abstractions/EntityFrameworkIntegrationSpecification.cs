using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore;

public abstract class EntityFrameworkIntegrationSpecification : Specification
{
    protected readonly EntityFrameworkCoreWebApplicationFactory _factory;

    protected EntityFrameworkIntegrationSpecification(
        HostFixture hostFixture,
        EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper ) : base(testOutputHelper)
    {
        _factory = factory;
        _factory.HostFixture = hostFixture;
    }
}
