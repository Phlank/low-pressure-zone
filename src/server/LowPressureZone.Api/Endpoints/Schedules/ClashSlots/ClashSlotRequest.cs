namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

public class ClashSlotRequest
{
    public required Guid PerformerOneId { get; set; }
    public required Guid PerformerTwoId { get; set; }
    public List<string> Rounds { get; set; } = [];
    public required DateTimeOffset StartsAt { get; set; }
    public required int Duration { get; set; }
}