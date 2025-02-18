using System.Security.Claims;

namespace LowPressureZone.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool HasAnySpecifiedRole(this ClaimsPrincipal principal, params string[] roles)
    {
        if (principal == null
            || !principal.Identities.Any()
            || !principal.Identities.First().IsAuthenticated)
        {
            return false;
        }

        return principal.GetRoles().Intersect(roles).Any();
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        IEnumerable<string>? roles = principal.Identities.FirstOrDefault()?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        return roles ?? [];
    }
}
