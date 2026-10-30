using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.Common.ReminderValueObject;

public readonly record struct Reminder
{
    public DateTimeOffset? ReminderTime { get; private init; }
    public string? JobId { get; private init; }
    
    [Obsolete("Use the static Create method to create an instance.", true)]
    public Reminder() {}
    
    private Reminder(DateTimeOffset? reminderTime, string? jobId)
    {
        ReminderTime = reminderTime;
        JobId = jobId;
    }
    
    public static DomainResult<Reminder> Create(DateTimeOffset? reminderTime, string? jobId = null)
    {
        if (reminderTime.HasValue && reminderTime <= DateTimeOffset.UtcNow)
        {
            return DomainResult.Err<Reminder>(new RuleError("Reminder time must be in the future."));
        }

        return DomainResult.Ok(new Reminder(reminderTime, jobId));
    }
}