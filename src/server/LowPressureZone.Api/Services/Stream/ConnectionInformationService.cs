using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Options;
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
    private readonly StreamingOptions.StreamInstanceConfiguration _liveInfo = streamOptions.Value.LiveInfo;
    private readonly StreamingOptions.StreamInstanceConfiguration _testInfo = streamOptions.Value.TestInfo;

    public async Task<Result<StreamingInfo, string>> GetLiveInfoAsync()
    {
        var claimsPrincipal = httpContextAccessor.GetAuthenticatedUserOrDefault();
        if (claimsPrincipal is null) return Result.Err<StreamingInfo, string>("User not logged in");

        var user = await userManager.GetUserAsync(claimsPrincipal);
        if (user is null) return Result.Err<StreamingInfo, string>("Could not find user in store");

        var streamerResult = await userManager.GetStreamerAsync(user, azuraCastClient);
        if (!streamerResult.IsSuccess) return Result.Err<StreamingInfo, string>(streamerResult.Error!);

        var streamer = streamerResult.Value!;
        return Result.Ok<StreamingInfo, string>(new StreamingInfo
        {
            Host = _liveInfo.Connection.Host,
            Port = _liveInfo.Connection.Port,
            Mount = _liveInfo.Connection.Mount,
            Username = streamer.StreamerUsername,
            DisplayName = streamer.DisplayName,
            Type = _liveInfo.Connection.Type
        });
    }

    public StreamingInfo GetTestInfo() =>
        new()
        {
            Host = _testInfo.Connection.Host,
            Port = _testInfo.Connection.Port,
            Mount = _testInfo.Connection.Mount,
            Type = _testInfo.Connection.Type,
            Username = _testInfo.User!.Username,
            Password = _testInfo.User!.Password
        };
}