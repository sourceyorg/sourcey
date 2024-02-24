namespace Sourcey.Integration.Tests.InMemory;

[CollectionDefinition(nameof(InMemoryIntegrationCollection))]
public class InMemoryIntegrationCollection :
    ICollectionFixture<InMemoryWebApplicationFactory>;
