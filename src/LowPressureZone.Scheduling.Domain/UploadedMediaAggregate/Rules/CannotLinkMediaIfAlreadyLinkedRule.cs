using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class CannotLinkMediaIfAlreadyLinkedRule(UploadedMedia media, int azuraCastMediaId) : IRule
{
    public bool IsBroken() => media.AzuraCastMediaId.HasValue && media.AzuraCastMediaId.Value != azuraCastMediaId;

    public RuleError Error => new("Already linked, cannot change linked media", nameof(UploadedMedia.AzuraCastMediaId));
}