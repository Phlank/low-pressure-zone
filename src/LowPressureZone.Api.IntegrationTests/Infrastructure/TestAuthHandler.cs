using System.Text.Encodings.Web;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.IntegrationTests.Infrastructure;

/// <summary>
///     Authenticates requests as the user whose id is given in the <see cref="UserIdHeader" /> header. The principal is
///     built with the same claims factory used by the Identity cookie sign in, so role claims match production.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> claimsPrincipalFactory)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string UserIdHeader = "X-Test-User-Id";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserIdHeader, out var userId))
            return AuthenticateResult.NoResult();

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return AuthenticateResult.Fail($"Unknown test user '{userId}'.");

        var principal = await claimsPrincipalFactory.CreateAsync(user);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName));
    }
}
