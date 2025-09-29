using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistOrders
{
    [JsonStringEnumMemberName("random")] Random,
    [JsonStringEnumMemberName("shuffle")] Shuffle,

    [JsonStringEnumMemberName("sequential")]
    Sequential
}
