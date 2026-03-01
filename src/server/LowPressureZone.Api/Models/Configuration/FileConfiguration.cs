namespace LowPressureZone.Api.Models.Configuration;

public class FileConfiguration
{
    public const string Name = "Files";
    public required string AzuraCastPrerecordedSetLocation { get; set; }
}