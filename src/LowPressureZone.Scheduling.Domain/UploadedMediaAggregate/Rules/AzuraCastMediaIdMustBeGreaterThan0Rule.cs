using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class AzuraCastMediaIdMustBeGreaterThan0Rule(int id) : IRule
{
    public bool IsBroken() => id <= 0;

    public RuleError Error => new("AzuraCast media ID must be greater than 0", nameof(UploadedMedia.AzuraCastMediaId));
}