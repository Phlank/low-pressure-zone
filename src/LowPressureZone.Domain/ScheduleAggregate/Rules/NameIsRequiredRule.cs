using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class NameIsRequiredRule(string name) : IRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(name);

    public RuleError Error => new("Required", nameof(Schedule.Name));
}