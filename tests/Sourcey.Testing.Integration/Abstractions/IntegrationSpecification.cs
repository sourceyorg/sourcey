using Microsoft.AspNetCore.Mvc.Testing;
using Sourcey.Testing.Abstractions;
using Sourcey.Testing.Integration.Stubs;
using Xunit;
using Xunit.Abstractions;

namespace Sourcey.Testing.Integration.Abstractions;

public abstract class SourceyWebApplicationFactory : WebApplicationFactory<Program>
{
}

public abstract class IntegrationSpecification<TWebApplicationFactory> : Specification, IClassFixture<TWebApplicationFactory>
    where TWebApplicationFactory : SourceyWebApplicationFactory
{
    protected readonly TWebApplicationFactory _factory;

    public IntegrationSpecification(
        ITestOutputHelper testOutputHelper,
        TWebApplicationFactory factory) : base(testOutputHelper)
    {
        _factory = factory;
    }
}

public abstract class IntegrationSpecification<TWebApplicationFactory, TResult> : Specification<TResult>, IClassFixture<TWebApplicationFactory>
    where TWebApplicationFactory : SourceyWebApplicationFactory
{
    protected readonly TWebApplicationFactory _factory;

    public IntegrationSpecification(
        ITestOutputHelper testOutputHelper,
        TWebApplicationFactory factory) : base(testOutputHelper)
    {
        _factory = factory;
    }
}
