using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Stream.AzuraCast.Schema;
using LowPressureZone.Api.Services;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Extensions;

public static class UserManagerExtensions
{
    private static string NewStreamerPassword => Guid.NewGuid().ToString().Replace("-", "");

    public static async Task SendWelcomeEmail(this UserManager<AppUser> userManager,
                                              AppUser user,
                                              EmailService emailService)
    {
        var inviteToken = await userManager.GenerateUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite);
        var tokenContext = new TokenContext
        {
            Email = user.Email!,
            Token = inviteToken
        };
        await emailService.SendInviteEmailAsync(user.Email!, tokenContext);
    }

    public static async Task<Result<string, string>> LinkToNewStreamer(this UserManager<AppUser> userManager,
                                                                       AppUser user,
                                                                       AzuraCastClient client)
    {
        var displayName = user.DisplayName;
        var username = user.UserName;
        var password = Guid.NewGuid().ToString().Replace("-", "");

        if (user.StreamerId != null) return Result.Err<string, string>("User already linked to streamer");

        var createResult = await client.CreateStreamerAsync(username!, password, displayName);
        if (!createResult.IsSuccess)
            return
                Result.Err<string, string>($"Unable to create streamer for user: ${createResult.Error?.ReasonPhrase ?? "No reason given by AzuraCast"}");

        user.StreamerId = createResult.Value;
        await userManager.UpdateAsync(user);
        return Result.Ok<string, string>(password);
    }

    public static async Task<Result<bool, string>> LinkToExistingStreamer(this UserManager<AppUser> userManager,
                                                                          AppUser user,
                                                                          AzuraCastClient client)
    {
        var streamersResult = await client.GetStreamersAsync();
        if (!streamersResult.IsSuccess) return Result.Err<bool, string>("Unable to get streamers");

        var match = streamersResult.Value!.FirstOrDefault(streamer => streamer.StreamerUsername == user.UserName);
        if (match is null) return Result.Err<bool, string>("No streamer found with matching username");

        user.StreamerId = match.Id;
        await userManager.UpdateAsync(user);
        return Result.Ok<bool, string>(true);
    }

    public static async Task<Result<string, string>> GenerateStreamerPassword(this UserManager<AppUser> userManager,
                                                                              AppUser user,
                                                                              AzuraCastClient client)
    {
        if (user.StreamerId is null) return Result.Err<string, string>("User does not have linked streamer");

        var getStreamerResult = await client.GetStreamerAsync(user.StreamerId.Value);
        if (!getStreamerResult.IsSuccess)
            return Result.Err<string, string>($"Unable to get streamer: {getStreamerResult.Error!.ReasonPhrase}");

        var streamer = getStreamerResult.Value!;
        streamer.StreamerPassword = NewStreamerPassword;
        var updateStreamerResult = await client.UpdateStreamerAsync(streamer);
        if (updateStreamerResult.IsSuccess) return Result.Ok<string, string>(streamer.StreamerPassword);

        return
            Result.Err<string, string>($"Unable to save streamer password: {updateStreamerResult.Error!.ReasonPhrase}");
    }

    public static async Task<Result<Streamer, string>> GetStreamerAsync(this UserManager<AppUser> userManager,
                                                                        AppUser user,
                                                                        AzuraCastClient client)
    {
        if (!user.StreamerId.HasValue)
            return Result.Err<Streamer, string>("User is not linked to streamer");

        var getStreamerResult = await client.GetStreamerAsync(user.StreamerId.Value);
        if (!getStreamerResult.IsSuccess)
            return Result.Err<Streamer, string>($"Unable to get streamer: {getStreamerResult.Error!.ReasonPhrase}");

        return Result.Ok<Streamer, string>(getStreamerResult.Value!);
    }
}
