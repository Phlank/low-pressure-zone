namespace LowPressureZone.Domain.Entities;

public class Broadcast : BaseEntity
{
    public int AzuraCastBroadcastId { get; set; }
    public bool IsArchived { get; set; }
}