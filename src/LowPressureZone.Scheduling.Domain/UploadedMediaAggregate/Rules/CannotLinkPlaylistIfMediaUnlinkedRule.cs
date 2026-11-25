using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class CannotLinkPlaylistIfMediaUnlinkedRule(UploadedMedia uploadedMedia) : IRule
{
    public bool IsBroken() => uploadedMedia.AzuraCastMediaId is null;

    public RuleError Error => new("Cannot link playlist if media is unlinked", nameof(UploadedMedia.AzuraCastMediaId));
}