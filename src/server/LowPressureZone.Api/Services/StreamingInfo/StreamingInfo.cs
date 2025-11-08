namespace LowPressureZone.Api.Services.StreamingInfo;

public sealed class StreamingInfo
{
    public required AzuraCastStreamingInfo Live { get; init; }
    public required IcecastStreamingInfo Test { get; init; }
}
