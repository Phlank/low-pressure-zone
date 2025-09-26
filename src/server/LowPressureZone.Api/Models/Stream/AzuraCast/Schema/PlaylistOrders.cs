using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistOrders
{
    [JsonPropertyName("random")] Random,
    [JsonPropertyName("shuffle")] Shuffle,
    [JsonPropertyName("sequential")] Sequential
}
