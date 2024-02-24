using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.EntityFrameworkCore;

public abstract class EntityFrameworkIntegrationSpecification : Specification
{
    protected readonly EntityFrameworkCoreWebApplicationFactory _factory;

    protected EntityFrameworkIntegrationSpecification(
        ProjectionsDbFixture projectionsDbFixture,
        EventStoreDbFixture eventStoreDbFixture,
        EntityFrameworkCoreWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper ) : base(testOutputHelper)
    {
        _factory = factory;
        _factory.eventStore = eventStoreDbFixture;
        _factory.projections = projectionsDbFixture;
    }
}
