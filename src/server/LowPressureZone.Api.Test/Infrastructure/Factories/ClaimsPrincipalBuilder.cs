using System.Security.Claims;

namespace LowPressureZone.Api.Test.Infrastructure.Factories;

public sealed class ClaimsPrincipalBuilder
{
    private readonly List<string> roles = [];
    private bool _hasClaimsCheckedClaim;
    private bool _isAuthenticated;
    private string _userId = "";

    public ClaimsPrincipal Build()
    {
        var principal = new ClaimsPrincipal();
        var identity = new ClaimsIdentity(_isAuthenticated ? "TestAuthentication" : null);
        if (!string.IsNullOrEmpty(_userId))
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, _userId));

        foreach (var role in roles)
            identity.AddClaim(new Claim(ClaimTypes.Role, role));

        if (_hasClaimsCheckedClaim)
            identity.AddClaim(new Claim("AdditionalClaimsChecked", "true"));

        principal.AddIdentity(identity);
        return principal;
    }

    public ClaimsPrincipalBuilder WithUserId(string userId)
    {
        _userId = userId;
        return this;
    }

    public ClaimsPrincipalBuilder AsAuthenticated()
    {
        _isAuthenticated = true;
        return this;
    }

    public ClaimsPrincipalBuilder WithRole(string role)
    {
        roles.Add(role);
        return this;
    }

    public ClaimsPrincipalBuilder WithClaimsCheckedClaim()
    {
        _hasClaimsCheckedClaim = true;
        return this;
    }
}