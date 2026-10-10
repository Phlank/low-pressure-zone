using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class NameLengthCannotExceed256Rule(string name) : IRule
{
    public bool IsBroken() => name.Trim().Length > 256;

    public RuleError Error => new("Name cannot exceed 256 characters", nameof(Schedule.Name));
}