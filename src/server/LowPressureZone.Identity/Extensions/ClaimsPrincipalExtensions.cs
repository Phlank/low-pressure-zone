using System.Security.Claims;

namespace LowPressureZone.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetIdOrDefault(this ClaimsPrincipal principal)
    {
        var id = principal.Identities.FirstOrDefault()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                          ?.Value;
        return id == null ? Guid.Empty : new Guid(id);
    }
}