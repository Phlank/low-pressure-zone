using System.Text.Json.Serialization;
using LowPressureZone.Api.JsonConverters;

namespace LowPressureZone.Api.Models.Stream.Icecast;

public class IcecastStatusRaw
{
    private const int SecondsUntilStale = 30;
    private readonly DateTime _retrievedAt = DateTime.UtcNow;
    public bool IsStale => DateTime.UtcNow - _retrievedAt > TimeSpan.FromSeconds(SecondsUntilStale);

    [JsonPropertyName("admin")] public required string Admin { get; init; }

    [JsonPropertyName("host")] public required string Host { get; init; }

    [JsonPropertyName("location")] public required string Location { get; init; }

    [JsonPropertyName("server_id")] public required string ServerId { get; init; }

    [JsonPropertyName("server_start")] public required string ServerStart { get; init; }

    [JsonPropertyName("server_start_iso8601")]
    [JsonConverter(typeof(Iso86012004DateTimeConverter))]
    public DateTime ServerStartIso8601 { get; init; }

    [JsonPropertyName("source")]
    [JsonConverter(typeof(ToEnumerableJsonConverter<IcecastSourceRaw>))]
    public IEnumerable<IcecastSourceRaw> Sources { get; init; } = [];
}