using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.ScheduleAggregate.Rules;

public class DescriptionLengthCannotExceed16384Rule(string description) : IRule
{
    public bool IsBroken() => description.Trim().Length > 16384;

    public RuleError Error => new("Description cannot exceed 16384", nameof(Schedule.Description));
}