using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.Icecast;

public class IcecastStatusRootRaw
{
    [JsonPropertyName("icestats")] public required IcecastStatusRaw Stats { get; set; }
}