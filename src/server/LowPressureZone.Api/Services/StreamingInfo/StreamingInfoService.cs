using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Core;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services.StreamingInfo;

public sealed class StreamingInfoService(
    IOptions<StreamingOptions> streamOptions,
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager,
    AzuraCastClient azuraCastClient)
{
    private readonly StreamConnection _liveInfo =
        streamOptions.Value.Live;

    private readonly StreamConnection _testInfo =
        streamOptions.Value.Test;

    public async Task<Result<Services.StreamingInfo.StreamingInfo, string>> GetInfoAsync()
    {
        var liveResult = await GetAzuracastInfoAsync(_liveInfo);
        if (!liveResult.IsSuccess)
            return Result.Err<Services.StreamingInfo.StreamingInfo, string>(liveResult.Error);
        
        var test = GetIcecastInfo(_testInfo);
        return Result.Ok<Services.StreamingInfo.StreamingInfo, string>(new()
        {
            Test = test,
            Live = liveResult.Value
        });
    }

    private async Task<Result<AzuraCastStreamingInfo, string>> GetAzuracastInfoAsync(StreamConnection connection)
    {
        var claimsPrincipal = httpContextAccessor.GetAuthenticatedUserOrDefault();
        if (claimsPrincipal is null) return Result.Err<AzuraCastStreamingInfo, string>("User not logged in");

        var user = await userManager.GetUserAsync(claimsPrincipal);
        if (user is null) return Result.Err<AzuraCastStreamingInfo, string>("Could not find user in store");

        var streamerResult = await userManager.GetStreamerAsync(user, azuraCastClient);
        if (!streamerResult.IsSuccess) return Result.Err<AzuraCastStreamingInfo, string>(streamerResult.Error);

        var streamer = streamerResult.Value;
        return Result.Ok<AzuraCastStreamingInfo, string>(new AzuraCastStreamingInfo
        {
            Host = connection.Host,
            Port = connection.Port,
            Mount = connection.Mount,
            Username = streamer.StreamerUsername,
            DisplayName = streamer.DisplayName ?? streamer.StreamerUsername
        });
    }

    private static IcecastStreamingInfo GetIcecastInfo(StreamConnection connection) =>
        new()
        {
            Host = connection.Host,
            Port = connection.Port,
            Mount = connection.Mount,
            Username = connection.Credentials?.Username,
            Password = connection.Credentials?.Password,
        };
}