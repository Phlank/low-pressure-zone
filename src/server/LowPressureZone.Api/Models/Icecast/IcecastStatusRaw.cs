using System.Text.Json.Serialization;
using LowPressureZone.Api.JsonConverters;

namespace LowPressureZone.Api.Models.Icecast;

public class IcecastStatusRaw
{
    public string Admin { get; set; }
    public string Host { get; set; }
    public string Location { get; set; }

    [JsonPropertyName("server_id")]
    public string ServerId { get; set; }

    [JsonPropertyName("server_start")]
    public string ServerStart { get; set; }

    [JsonPropertyName("server_start_iso8601")]
    public DateTime ServerStartIso8601 { get; set; }

    [JsonPropertyName("source")]
    [JsonConverter(typeof(ObjectOrArrayJsonConverter<IcecastSourceRaw>))]
    public IEnumerable<IcecastSourceRaw> Sources { get; set; }
}
