using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.Common.ReminderValueObject.Rules;

public class ReminderMustBeInFuture(DateTimeOffset? reminderTime) : IRule
{
    public bool IsBroken() => reminderTime.HasValue && reminderTime <= DateTimeOffset.UtcNow;

    public RuleError Error => new("Time must be in the future", nameof(Reminder.ReminderTime));
}