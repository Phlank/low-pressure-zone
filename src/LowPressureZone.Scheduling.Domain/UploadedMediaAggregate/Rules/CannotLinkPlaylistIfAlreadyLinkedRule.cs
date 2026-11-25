using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class CannotLinkPlaylistIfAlreadyLinkedRule(UploadedMedia uploadedMedia) : IRule
{
    public bool IsBroken() => uploadedMedia.AzuraCastPlaylistId.HasValue;

    public RuleError Error => new("Already linked, cannot change linked playlist", nameof(UploadedMedia.AzuraCastPlaylistId));
}