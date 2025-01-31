namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required DateTime CreatedDate { get; set; }
    public required DateTime ModifiedDate { get; set; }
}
