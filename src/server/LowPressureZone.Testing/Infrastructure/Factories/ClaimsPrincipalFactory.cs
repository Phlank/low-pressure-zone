using System.Security.Claims;

namespace LowPressureZone.Testing.Infrastructure.Factories;

public static class ClaimsPrincipalFactory
{
    private const string AdditionalClaimsCheckedClaimType = "AdditionalClaimsChecked";

    public static ClaimsPrincipal Create(
        Guid? userId = null,
        IEnumerable<string>? roles = null,
        bool additionalClaimsChecked = false,
        IEnumerable<Claim>? additionalClaims = null)
    {
        roles ??= [];
        additionalClaims ??= [];

        var principal = new ClaimsPrincipal();
        var identity = new ClaimsIdentity(userId is not null ? "TestAuthentication" : null);

        if (userId.HasValue)
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));

        foreach (var role in roles)
            identity.AddClaim(new Claim(ClaimTypes.Role, role));

        if (additionalClaimsChecked)
            identity.AddClaim(new Claim(AdditionalClaimsCheckedClaimType, "true"));

        foreach (var claim in additionalClaims)
            identity.AddClaim(claim);

        principal.AddIdentity(identity);
        return principal;
    }

    public static ClaimsPrincipal Anonymous() => Create();
}