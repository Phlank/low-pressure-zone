using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Models.Stream;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services.Stream;

public class StreamInformationService(IOptions<StreamingOptions> streamOptions)
{
    private readonly StreamingInfo _liveInfo = streamOptions.Value.LiveInfo;
    private readonly StreamingInfo _testInfo = streamOptions.Value.TestInfo;

    public StreamingInfo GetLiveInfo() => _liveInfo;
    public StreamingInfo GetTestInfo() => _testInfo;
}
