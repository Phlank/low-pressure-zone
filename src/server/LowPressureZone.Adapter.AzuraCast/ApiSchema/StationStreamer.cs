using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public sealed record StationStreamer
{
    public int Id { get; set; }

    [JsonPropertyName("streamer_username")]
    public required string StreamerUsername { get; set; }

    [JsonPropertyName("streamer_password")]
    public string StreamerPassword { get; set; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    public string? Comments { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("enforce_schedule")]
    public bool EnforceSchedule { get; set; }

    [JsonPropertyName("reactivate_at")]
    public int? ReactivateAt { get; set; }

    [JsonPropertyName("schedule_items")]
    public IReadOnlyCollection<string> ScheduleItems { get; set; } = [];
}
