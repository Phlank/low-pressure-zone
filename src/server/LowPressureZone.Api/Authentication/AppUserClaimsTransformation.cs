using System.Security.Claims;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Authentication;

public sealed class AppUserClaimsTransformation(DataContext dataContext) : IClaimsTransformation
{
    private const string AdditionalClaimsCheckedClaimType = "AdditionalClaimsChecked";

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (!principal.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
            return principal;

        if (principal.HasClaim(claim => claim.Type == AdditionalClaimsCheckedClaimType))
            return principal;

        var identity = new ClaimsIdentity();
        var relationshipRoles = await dataContext.CommunityRelationships
                                           .Where(relationship => relationship.UserId == principal.GetIdOrDefault())
                                           .Select(relationship => new
                                                       { relationship.IsOrganizer, relationship.IsPerformer })
                                           .ToListAsync();
        
        if (relationshipRoles.Any(role => role.IsOrganizer))
            identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Organizer));
        
        if (relationshipRoles.Any(role => role.IsPerformer))
            identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Performer));
        
        identity.AddClaim(new Claim(AdditionalClaimsCheckedClaimType, "true"));
        
        principal.AddIdentity(identity);
        return principal;
    }
}