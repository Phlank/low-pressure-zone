using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models.Configuration.Streaming;
using LowPressureZone.Core;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services.StreamConnectionInfo;

public sealed class StreamingInfoService(
    IOptions<StreamingConfiguration> streamOptions,
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager,
    AzuraCastClient azuraCastClient)
{
    private readonly AzuraCastStreamConfiguration _liveConfiguration =
        streamOptions.Value.Live;

    private readonly IcecastStreamConfiguration _testConfiguration =
        streamOptions.Value.Test;

    public async Task<Result<StreamingInfo, string>> GetInfoAsync()
    {
        var liveResult = await GetAzuracastInfoAsync(_liveConfiguration);
        if (!liveResult.IsSuccess)
            return Result.Err<StreamingInfo, string>(liveResult.Error);

        var test = GetIcecastInfo(_testConfiguration);
        return Result.Ok<StreamingInfo, string>(new StreamingInfo
        {
            Live = liveResult.Value,
            Test = test
        });
    }

    private async Task<Result<AzuraCastStreamingInfo, string>> GetAzuracastInfoAsync(
        AzuraCastStreamConfiguration configuration)
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
            Host = configuration.Host,
            Port = configuration.Port,
            Mount = configuration.Mount,
            Username = streamer.StreamerUsername,
            DisplayName = streamer.DisplayName ?? streamer.StreamerUsername
        });
    }

    private static IcecastStreamingInfo GetIcecastInfo(IcecastStreamConfiguration configuration) =>
        new()
        {
            Host = configuration.Host,
            Port = configuration.Port,
            Mount = configuration.Mount,
            Username = configuration.Username,
            Password = configuration.Password
        };
}