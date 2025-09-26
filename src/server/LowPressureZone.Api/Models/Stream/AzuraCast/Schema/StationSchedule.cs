namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public class StationSchedule
{
    public int Id { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }
    public ICollection<int> Days { get; set; }
}
