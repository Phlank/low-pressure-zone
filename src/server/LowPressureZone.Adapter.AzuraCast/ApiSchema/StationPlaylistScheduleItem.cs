namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationPlaylistScheduleItem
{
    public int StartTime { get; set; }
    public int EndTime { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    /// <summary>
    /// An array of integers representing the days of the week the schedule item is active.
    /// 0 = Sunday, 
    /// </summary>
    public IEnumerable<int> Days { get; set; } = [];
    public bool LoopOnce { get; set; }
}