using System.Security.Claims;

namespace LowPressureZone.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetIdOrDefault(this ClaimsPrincipal principal)
    {
        var id = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        return id == null ? Guid.Empty : new Guid(id);
    }
}