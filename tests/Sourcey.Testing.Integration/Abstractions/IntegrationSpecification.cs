using Microsoft.AspNetCore.Mvc.Testing;
using Sourcey.Testing.Abstractions;
using Sourcey.Testing.Integration.Stubs;
using Xunit;
using Xunit.Abstractions;

namespace Sourcey.Testing.Integration.Abstractions;

public abstract class SourceyWebApplicationFactory : WebApplicationFactory<Program>
{
}
