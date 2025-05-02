using System.Security.Claims;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Authentication;

public class AppUserClaimsTransformation(DataContext dataContext) : IClaimsTransformation
{
    private const string OrganizerCheckedClaimType = "OrganizerChecked";

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (!principal.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
            return principal;

        if (principal.HasClaim(claim => claim.Type == OrganizerCheckedClaimType))
            return principal;

        var identity = new ClaimsIdentity();
        var isOrganizer = await dataContext.CommunityRelationships
                                           .AsNoTracking()
                                           .AnyAsync(relationship => relationship.IsOrganizer && relationship.UserId == principal.GetIdOrDefault());
        identity.AddClaim(new Claim(OrganizerCheckedClaimType, "true"));
        if (isOrganizer)
            identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Organizer));
        principal.AddIdentity(identity);
        return principal;
    }
}
