using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Scheduling.Domain.Common.ReminderValueObject;
using LowPressureZone.Scheduling.Domain.PerformerAggregate;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixEntity;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.Rules;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.Rules;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;

[Table("HourlySlots")]
public class HourlySlot : Entity
{
    public Guid ScheduleId { get; private init; }
    public Schedule Schedule { get; init; } = null!;

    [MaxLength(128)]
    public string? Subtitle { get; private set; }

    public Guid PerformerId { get; private set; } = Guid.Empty;
    public Performer Performer { get; private init; } = null!;
    public Guid? PrerecordedMixId { get; private set; }
    public PrerecordedMix? PrerecordedMix { get; private set; }
    public HourlySlotTimeRange TimeRange { get; private set; }
    public Reminder Reminder { get; private set; }

    private HourlySlot()
    {
    }

    public static DomainResult<HourlySlot> Create(
        Guid scheduleId,
        Guid performerId,
        DateTimeOffset startsAt,
        int duration,
        string? subtitle = null,
        string? uploadedFileName = null,
        Guid? uploadedMediaId = null,
        DateTimeOffset? reminderTime = null,
        Guid? id = null)
    {
        var value = new HourlySlot
        {
            Id = id ?? Guid.NewGuid(),
            ScheduleId = scheduleId,
        };

        var composed = DomainResult.ComposeInto(value,
                                                value.ChangeSubtitle(subtitle),
                                                value.ChangePerformer(performerId),
                                                value.ChangeTime(startsAt, duration),
                                                value.SetReminder(reminderTime));
        
        if (uploadedFileName is not null && uploadedMediaId.HasValue)
            composed = composed.ComposeWith(value.SetUpPrerecordedMix(uploadedMediaId.Value, uploadedFileName));
         
        return composed;
   }

    public DomainResult<NoValue> ChangeSubtitle(string? subtitle)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new SubtitleLengthCannotExceed128Rule(subtitle));
        if (result.IsSuccess)
            Subtitle = subtitle;

        return result;
    }

    public DomainResult<NoValue> ChangePerformer(Guid performerId)
    {
        PerformerId = performerId;
        return DomainResult.Ok();
    }

    public DomainResult<NoValue> ChangeTime(DateTimeOffset startsAt, int duration)
    {
        if (startsAt == TimeRange.StartsAt && duration == TimeRange.Duration)
            return DomainResult.Ok();

        var timeRangeResult = HourlySlotTimeRange.Create(startsAt, duration)
                                                 .WithAdditionalRules(new CannotChangeSlotAfterItEndsRule(TimeRange));

        if (timeRangeResult.IsSuccess)
            TimeRange = timeRangeResult.Value;

        return timeRangeResult.ToNoValue();
    }

    private DomainResult<NoValue> SetUpPrerecordedMix(Guid uploadedMediaId, string uploadedFileName)
    {
        var result = PrerecordedMix.Create(uploadedMediaId, uploadedFileName);
        if (result.IsSuccess)
            PrerecordedMix = result.Value;

        return result.ToNoValue();
    }

    public DomainResult<NoValue> SetReminder(DateTimeOffset? reminderTime)
    {
        if (Reminder.ReminderTime == reminderTime)
            return DomainResult.Ok();

        var @event = new HourlySlotReminderSet(Id, Reminder.JobId, reminderTime);

        var createResult = Reminder.Create(reminderTime);
        if (createResult.IsError)
            return createResult.ToNoValue();

        Reminder = createResult.Value;
        return DomainResult.Ok().WithAdditionalEvents(@event);
    }

    public void LinkReminder(string jobId)
    {
        if (!Reminder.ReminderTime.HasValue)
            throw new InvalidOperationException("Cannot link a reminder when no reminder time is set.");

        Reminder = Reminder.Create(Reminder.ReminderTime, jobId).Value;
    }

    public DomainResult<NoValue> Delete()
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new HourlySlotDeleted(Id, Reminder.JobId),
                                          new CannotChangeSlotAfterItEndsRule(TimeRange));
        if (result.IsSuccess)
        {
            Schedule.HourlySlots.Remove(this);
        }

        return result;
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HourlySlot>()
                    .Property(slot => slot.Id)
                    .ValueGeneratedNever();

        modelBuilder.Entity<HourlySlot>()
                    .ComplexProperty(slot => slot.TimeRange);

        modelBuilder.Entity<HourlySlot>()
                    .HasIndex(slot => slot.TimeRange.StartsAt)
                    .IsUnique();

        modelBuilder.Entity<HourlySlot>().HasOne(slot => slot.PrerecordedMix).WithOne(mix => mix.Slot)
                    .HasForeignKey<HourlySlot>(slot => slot.PrerecordedMixId);
        modelBuilder.Entity<HourlySlot>().Navigation(slot => slot.PrerecordedMix).AutoInclude();

        modelBuilder.Entity<HourlySlot>().HasOne(slot => slot.Performer).WithMany()
                    .HasForeignKey(slot => slot.PerformerId).ExcludeForeignKeyFromMigrations();
        modelBuilder.Entity<HourlySlot>().Navigation(slot => slot.Performer).AutoInclude();

        modelBuilder.Entity<HourlySlot>().ComplexProperty(slot => slot.Reminder);
    }
}