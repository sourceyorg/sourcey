using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sourcey.Testing.Integration.Abstractions
{
    internal sealed class AuthenticatedAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IList<Claim> _claims;
        internal const string AuthenticationScheme = "SourceyTestAuthenticationScheme";

        public AuthenticatedAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
                               UrlEncoder encoder, ISystemClock clock, ClaimsProvider claimsProvider) : base(options, logger, encoder, clock)
        {
            _claims = claimsProvider.Claims;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(_claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
