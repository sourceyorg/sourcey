using System.Security.Claims;

namespace Sourcey.Testing.Integration.Abstractions
{
    internal sealed class ClaimsProvider
    {
        public IList<Claim> Claims { get; }

        public ClaimsProvider(IList<Claim> claims)
        {
            Claims = claims;
        }

        public ClaimsProvider()
        {
            Claims = new List<Claim>();
        }
    }
}
