using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class AzuraCastPlaylistIdMustBeGreaterThan0Rule(int playlistId) : IRule
{
    public bool IsBroken() => playlistId <= 0;

    public RuleError Error =>
        new("AzuraCast playlist ID must be greater than 0", nameof(UploadedMedia.AzuraCastPlaylistId));
}