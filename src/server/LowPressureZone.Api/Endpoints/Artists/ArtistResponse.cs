namespace LowPressureZone.Api.Endpoints.Artists;

public class ArtistResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? PromoUrl { get; set; }
}