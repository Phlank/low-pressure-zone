namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public record Song
{
    public string Text { get; set; } = string.Empty;
    public string? Artist { get; set; }
    public string? Title { get; set; }
    public string? Album { get; set; }
    public string? Genre { get; set; }
    public string? Isrc { get; set; }
    public string? Lyrics { get; set; }
    public string Id { get; set; } = string.Empty;
    public string? Art { get; set; }
}
