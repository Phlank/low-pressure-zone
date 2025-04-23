using System.Text.Json.Serialization;
using LowPressureZone.Api.JsonConverters;

namespace LowPressureZone.Api.Models.Icecast;

public class IcecastStatusRaw
{
    public required string Admin { get; set; }
    public required string Host { get; set; }
    public required string Location { get; set; }

    [JsonPropertyName("server_id")]
    public required string ServerId { get; set; }

    [JsonPropertyName("server_start")]
    public required string ServerStart { get; set; }

    [JsonPropertyName("server_start_iso8601")]
    public DateTime ServerStartIso8601 { get; set; }

    [JsonPropertyName("source")]
    [JsonConverter(typeof(ToArrayJsonConverter<IcecastSourceRaw>))]
    public IEnumerable<IcecastSourceRaw> Sources { get; set; } = [];
}
