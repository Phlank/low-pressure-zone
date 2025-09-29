using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaylistSources
{
    [JsonStringEnumMemberName("songs")] Songs,

    [JsonStringEnumMemberName("remote_url")]
    RemoteUrl
}
