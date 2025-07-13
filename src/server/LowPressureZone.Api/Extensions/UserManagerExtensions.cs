using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Models;
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

    public static async Task<Result<string, string>> LinkToNewStreamer(this UserManager<AppUser> userManager, AppUser user, AzuraCastClient client)
    {
        var displayName = user.DisplayName;
        var username = user.UserName;
        var password = Guid.NewGuid().ToString().Replace("-", "");

        if (user.StreamerId != null) return new Result<string, string>(null, "User already linked to streamer");

        var createResult = await client.CreateStreamerAsync(username!, password, displayName);
        if (!createResult.IsSuccess) return new Result<string, string>(null, $"Unable to create streamer for user: ${createResult.Error?.ReasonPhrase ?? "No reason given by AzuraCast"}");

        user.StreamerId = createResult.Data;
        await userManager.UpdateAsync(user);
        return new Result<string, string>(password, null);
    }

    public static async Task<Result<string, string>> LinkToExistingStreamer(this UserManager<AppUser> userManager, AppUser user, AzuraCastClient client)
    {
        var password = Guid.NewGuid().ToString().Replace("-", "");
        var streamersResult = await client.GetStreamersAsync();
        if (!streamersResult.IsSuccess) return new Result<string, string>(null, "Unable to get streamers");

        var match = streamersResult.Data!.FirstOrDefault(streamer => streamer.StreamerUsername == user.UserName);
        if (match is null) return new Result<string, string>(null, "No streamer found with matching username");

        match.StreamerPassword = password;

        user.StreamerId = match.Id;
        await userManager.UpdateAsync(user);
        return new Result<string, string>(password, null);
    }
}
