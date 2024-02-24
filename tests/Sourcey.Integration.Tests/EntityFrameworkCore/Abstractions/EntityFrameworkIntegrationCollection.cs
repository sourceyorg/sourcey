namespace Sourcey.Integration.Tests.EntityFrameworkCore;

[CollectionDefinition(nameof(EntityFrameworkIntegrationCollection))]
public class EntityFrameworkIntegrationCollection :
    ICollectionFixture<ProjectionsDbFixture>,
    ICollectionFixture<EventStoreDbFixture>,
    ICollectionFixture<EntityFrameworkCoreWebApplicationFactory>;
