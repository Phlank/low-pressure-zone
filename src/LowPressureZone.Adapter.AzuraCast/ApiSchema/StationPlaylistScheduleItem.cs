using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public sealed class StationPlaylistScheduleItem
{
    [JsonPropertyName("start_time")]
    public int StartTime { get; set; }

    [JsonPropertyName("end_time")]
    public int EndTime { get; set; }

    [JsonPropertyName("start_date")]
    public DateOnly StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateOnly EndDate { get; set; }

    /// <summary>
    ///     An array of integers representing the days of the week the schedule item is active.
    ///     0 = Sunday, 6 = Saturday
    /// </summary>
    public IEnumerable<int> Days { get; set; } = [];

    [JsonPropertyName("loop_once")]
    public bool LoopOnce { get; set; }
}