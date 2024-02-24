using Microsoft.AspNetCore.Mvc.Testing;
using Sourcey.Testing.Integration.Stubs;

namespace Sourcey.Testing.Integration.Abstractions;

public abstract class SourceyWebApplicationFactory : WebApplicationFactory<Program>
{
}
