using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public sealed class StationMediaExtraMetadata
{
    public int? Amplify { get; set; }

    [JsonPropertyName("cross_start_next")]
    public int? CrossStartNext { get; set; }

    [JsonPropertyName("cue_in")]
    public int? CueIn { get; set; }

    [JsonPropertyName("cue_out")]
    public int? CueOut { get; set; }

    [JsonPropertyName("fade_in")]
    public int? FadeIn { get; set; }

    [JsonPropertyName("fade_out")]
    public int? FadeOut { get; set; }
}
