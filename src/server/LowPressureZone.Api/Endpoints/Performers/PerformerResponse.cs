namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required bool IsDeletable { get; set; }
    public required bool IsLinkable { get; set; }
}
