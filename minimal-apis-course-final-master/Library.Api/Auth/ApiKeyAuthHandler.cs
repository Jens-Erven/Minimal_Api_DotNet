﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Library.Api.Auth
{
    public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
    {

        public ApiKeyAuthHandler(
                 IOptionsMonitor<ApiKeyAuthSchemeOptions> options,
                 ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Api key"));
            }

            var claims = new[] {
                new Claim(ClaimTypes.Email, "jens.erven@euri.com"),
                new Claim(ClaimTypes.Name, "Jens Erven")};

            var claimsIdentity = new ClaimsIdentity(claims, "ApiKey");

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(claimsIdentity), Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));

        }
    }
}
