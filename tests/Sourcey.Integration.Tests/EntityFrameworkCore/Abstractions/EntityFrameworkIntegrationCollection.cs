namespace Sourcey.Integration.Tests.EntityFrameworkCore;

[CollectionDefinition(nameof(EntityFrameworkIntegrationCollection))]
public class EntityFrameworkIntegrationCollection :
    ICollectionFixture<HostFixture>,
    ICollectionFixture<EntityFrameworkCoreWebApplicationFactory>;
