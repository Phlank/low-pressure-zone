using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Extensions;

public static class UserManagerExtensions
{
    public static async Task SendWelcomeEmail(this UserManager<AppUser> userManager, AppUser user, EmailService emailService)
    {
        var inviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var tokenContext = new TokenContext
        {
            Email = user.Email!,
            Token = inviteToken
        };
        await emailService.SendInviteEmailAsync(user.Email!, tokenContext);
    }
}
