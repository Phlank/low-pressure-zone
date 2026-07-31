namespace LowPressureZone.Api.Endpoints.Artists;

public class ArtistRequest
{
    public required string Name { get; set; }
    public string? PromoUrl { get; set; }
}