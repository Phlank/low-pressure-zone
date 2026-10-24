namespace LowPressureZone.Api.Models.Configuration;

public class AzuraCastInstallationConfiguration
{
    public const string Name = "AzuraCastInstallation";
    public required string PrerecordedSetLocation { get; init; }
    public required string ArchiveSetLocation { get; init; }
    public required string ArchivePlaylistName { get; init; }
}