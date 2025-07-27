using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Api.Models.Stream.Info;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services.Stream;

public class ConnectionInformationService(
    IOptions<StreamingOptions> streamOptions,
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager,
    AzuraCastClient azuraCastClient)
{
    private readonly StreamInstanceOptions _liveInfo = streamOptions.Value.Live;
    private readonly StreamInstanceOptions _testInfo = streamOptions.Value.Test;

    public Task<Result<StreamingInfo, string>> GetLiveInfoAsync() => GetInfo(_liveInfo);

    public Task<Result<StreamingInfo, string>> GetTestInfoAsync() => GetInfo(_testInfo);

    private async Task<Result<StreamingInfo, string>> GetInfo(
        StreamInstanceOptions options) =>
        options.Connection.Type switch
        {
            StreamServerType.Icecast => GetIcecastInfo(options),
            StreamServerType.AzuraCast => await GetAzuracastInfoAsync(options),
            _ => throw new InvalidOperationException()
        };

    private async Task<Result<StreamingInfo, string>> GetAzuracastInfoAsync(
        StreamInstanceOptions options)
    {
        var claimsPrincipal = httpContextAccessor.GetAuthenticatedUserOrDefault();
        if (claimsPrincipal is null) return Result.Err<StreamingInfo, string>("User not logged in");

        var user = await userManager.GetUserAsync(claimsPrincipal);
        if (user is null) return Result.Err<StreamingInfo, string>("Could not find user in store");

        var streamerResult = await userManager.GetStreamerAsync(user, azuraCastClient);
        if (!streamerResult.IsSuccess) return Result.Err<StreamingInfo, string>(streamerResult.Error);

        var streamer = streamerResult.Value;
        return Result.Ok<StreamingInfo, string>(new StreamingInfo
        {
            Host = options.Connection.Host,
            Port = options.Connection.Port,
            Mount = options.Connection.Mount,
            Username = streamer.StreamerUsername,
            DisplayName = streamer.DisplayName,
            Type = options.Connection.Type
        });
    }

    private static Result<StreamingInfo, string> GetIcecastInfo(
        StreamInstanceOptions options) =>
        Result.Ok<StreamingInfo, string>(new StreamingInfo
        {
            Host = options.Connection.Host,
            Port = options.Connection.Port,
            Mount = options.Connection.Mount,
            Type = options.Connection.Type,
            Username = options.User?.Username,
            Password = options.User?.Password,
            DisplayName = options.User?.DisplayName
        });
}
