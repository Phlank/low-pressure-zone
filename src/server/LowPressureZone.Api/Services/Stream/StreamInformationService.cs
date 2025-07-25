using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services.Stream;

public class StreamInformationService(
    IOptions<StreamingOptions> streamOptions,
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager)
{
    private readonly StreamingInfo _liveInfo = streamOptions.Value.LiveInfo;
    private readonly StreamingInfo _testInfo = streamOptions.Value.TestInfo;

    public StreamingInfo GetLiveInfo()
    {
        var username = httpContextAccessor.HttpContext?.User.Identity?.Name;
        // TODO Get user's azuracast streamer
        return new StreamingInfo
        {
            Host = _liveInfo.Host,
            Port = _liveInfo.Port,
            Mount = _liveInfo.Mount,
            Username = username,
            Password = _liveInfo.Password,
            Type = _liveInfo.Type
        };
    }

    public StreamingInfo GetTestInfo() => _testInfo;
}
