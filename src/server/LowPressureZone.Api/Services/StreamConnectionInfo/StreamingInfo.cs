using System.Diagnostics.CodeAnalysis;

namespace LowPressureZone.Api.Services.StreamConnectionInfo;

public sealed class StreamingInfo
{
    public required AzuraCastStreamingInfo Live { get; init; }
    public required IcecastStreamingInfo Test { get; init; }
}
