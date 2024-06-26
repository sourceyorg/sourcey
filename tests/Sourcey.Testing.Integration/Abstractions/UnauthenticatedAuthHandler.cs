﻿using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Sourcey.Testing.Integration.Abstractions;

internal sealed class UnauthenticatedAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public UnauthenticatedAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = AuthenticateResult.Fail(new Exception("Unauthenticated"));
        return Task.FromResult(result);
    }
}
