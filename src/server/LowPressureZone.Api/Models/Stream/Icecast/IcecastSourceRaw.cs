using System.Text.Json.Serialization;
using LowPressureZone.Api.JsonConverters;

namespace LowPressureZone.Api.Models.Stream.Icecast;

public class IcecastSourceRaw
{
    public string? Artist { get; set; }
    public int? Bitrate { get; set; }
    public string? Genre { get; set; }
    public string? Subtype { get; set; }
    public string? Title { get; set; }

    [JsonPropertyName("audio_bitrate")] public int? AudioBitrate { get; set; }

    [JsonPropertyName("audio_channels")] public int? AudioChannels { get; set; }

    [JsonPropertyName("audio_info")] public string? AudioInfo { get; set; }

    [JsonPropertyName("audio_samplerate")] public int? AudioSampleRate { get; set; }

    [JsonPropertyName("ice-bitrate")] public int? IceBitrate { get; set; }

    [JsonPropertyName("listener_peak")] public required int ListenerPeak { get; set; }

    public required int Listeners { get; set; }

    [JsonPropertyName("listenurl")] public required string ListenUrl { get; set; }

    [JsonPropertyName("server_description")]
    public required string ServerDescription { get; set; }

    [JsonPropertyName("server_name")] public required string ServerName { get; set; }

    [JsonPropertyName("server_type")] public required string ServerType { get; set; }

    [JsonPropertyName("stream_start")] public required string StreamStart { get; set; }

    [JsonPropertyName("stream_start_iso8601")]
    [JsonConverter(typeof(Iso86012004DateTimeConverter))]
    public DateTime StreamStartIso8601 { get; set; }
}