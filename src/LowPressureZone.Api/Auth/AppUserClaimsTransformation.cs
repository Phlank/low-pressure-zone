using System.Security.Claims;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Auth;

public class AppUserClaimsTransformation(UserManager<AppUser> userManager, DataContext dataContext)
    : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var id = principal.GetIdOrDefault();

        if (id == Guid.Empty) return principal;

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return principal;

        var relationships = await dataContext.Communities
                                     .AsNoTracking()
                                     .Include(community => community.Relationships
                                                                    .Where(relationship =>
                                                                               relationship.UserId == user.Id
                                                                               && (relationship.IsOrganizer
                                                                                   || relationship.IsPerformer)))
                                     .SelectMany(community => community.Relationships)
                                     .ToListAsync();
        
        if (relationships.Count == 0) return principal;

        var isPerformer = relationships.Any(relationship => relationship.IsPerformer);
        var isOrganizer = relationships.Any(relationship => relationship.IsOrganizer);

        var communityRolesIdentity = new ClaimsIdentity();
        if (isPerformer)
            communityRolesIdentity.AddClaim(new(ClaimTypes.Role, RoleNames.Performer));

        if (isOrganizer)
            communityRolesIdentity.AddClaim(new(ClaimTypes.Role, RoleNames.Organizer));
        
        principal.AddIdentity(communityRolesIdentity);
        return principal;
    }
}