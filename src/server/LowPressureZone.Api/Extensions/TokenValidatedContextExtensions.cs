using System.Security.Claims;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Extensions;

public static class TokenValidatedContextExtensions
{
    public static string GetEmailClaimValue(this TokenValidatedContext context)
    {
        if (context.Principal == null) throw new Exception("No principal on token.");
        return context.Principal.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value ?? throw new Exception("No email claim on token.");
    }

    public static async Task<AppUser?> GetUser(this TokenValidatedContext context, string email)
    {
        var identityContext = context.HttpContext.RequestServices.GetRequiredService<IdentityContext>();
        var user = await identityContext.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
        return user;
    }

    public static async Task AddUserRolesToPrincipal(this TokenValidatedContext context, AppUser user)
    {
        var identityContext = context.HttpContext.RequestServices.GetRequiredService<IdentityContext>();
        var roleIds = await identityContext.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToListAsync();
        var roleNames = await identityContext.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync();
        foreach (var roleName in roleNames)
        {
            context.Principal!.Identities.First().AddClaim(new Claim(ClaimTypes.Role, roleName!));
        }
    }
}
