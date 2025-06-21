using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Models.Options;

public class StreamingOptions
{
    public const string Name = "Streaming";
    public required string ServerType { get; set; }
    public required StreamingInfo LiveInfo { get; set; }
    public required StreamingInfo TestInfo { get; set; }
}
