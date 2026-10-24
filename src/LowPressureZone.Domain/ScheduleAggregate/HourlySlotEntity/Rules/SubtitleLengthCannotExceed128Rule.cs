using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Rules;

public class SubtitleLengthCannotExceed128Rule(string? subtitle) : IRule
{
    public bool IsBroken() => !string.IsNullOrWhiteSpace(subtitle) && subtitle.Length > 128;

    public RuleError Error => new("Cannot exceed 128 characters", nameof(HourlySlot.Subtitle));
}