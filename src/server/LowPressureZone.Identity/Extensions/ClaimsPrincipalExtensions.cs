using System.Security.Claims;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool IsInAnyRole(this ClaimsPrincipal principal, params IEnumerable<string> roles)
    {
        if (!roles.Any()
            || principal.Identity == null
            || !principal.Identity.IsAuthenticated) return false;

        foreach (var role in roles)
        {
            if (principal.IsInRole(role)) return true;
        }
        return false;
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        var roles = principal.Identities.FirstOrDefault()?.Claims
                             .Where(c => c.Type == ClaimTypes.Role)
                             .Select(c => c.Value);
        return roles ?? [];
    }

    public static Guid GetIdOrDefault(this ClaimsPrincipal principal)
    {
        var id = principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return id == null ? Guid.Empty : new Guid(id);
    }

    public static string? GetNameOrDefault(this ClaimsPrincipal principal)
    {
        var name = principal.Identities.FirstOrDefault()?.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        if (name == null) return default;
        return name;
    }

    public static async Task<AppUser?> GetAppUserOrDefaultAsync(this ClaimsPrincipal principal, UserManager<AppUser> userManager)
    {
        var username = principal.Identity?.Name;
        if (username == null) return null;

        return await userManager.FindByNameAsync(username);
    }
}
