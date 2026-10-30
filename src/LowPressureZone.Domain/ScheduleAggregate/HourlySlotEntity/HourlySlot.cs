using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastEndpoints;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.Common.ReminderValueObject;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Events;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Rules;
using LowPressureZone.Domain.ScheduleAggregate.Rules;
using Microsoft.EntityFrameworkCore;
using IEvent = LowPressureZone.Core.Domain.IEvent;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;

[Table("HourlySlots")]
public class HourlySlot : Entity
{
    public Guid ScheduleId { get; private init; }
    public Schedule Schedule { get; init; } = null!;

    [MaxLength(128)]
    public string? Subtitle { get; private set; }

    public Guid PerformerId { get; private set; } = Guid.Empty;
    public Performer Performer { get; private init; } = null!;
    public PrerecordedMix Prerecord { get; private set; }
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
        int? azuraCastMediaId = null,
        DateTimeOffset? reminderTime = null,
        Guid? id = null)
    {
        var value = new HourlySlot
        {
            Id = id ?? Guid.NewGuid(),
            ScheduleId = scheduleId,
        };

        return DomainResult.ComposeInto(value,
                                        value.ChangeSubtitle(subtitle),
                                        value.ChangePerformer(performerId),
                                        value.ReplacePrerecordedMix(uploadedFileName, azuraCastMediaId),
                                        value.ChangeTime(startsAt, duration),
                                        value.SetReminder(reminderTime));
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

    public DomainResult<NoValue> ReplacePrerecordedMix(string? uploadedFileName, int? azuraCastMediaId = null)
    {
        if (Prerecord.UploadedFileName == uploadedFileName && Prerecord.AzuraCastMediaId == azuraCastMediaId)
            return DomainResult.Ok();
        
        DomainResult<PrerecordedMix> mixResult;
        if (uploadedFileName is null && azuraCastMediaId is null)
        {
            mixResult = PrerecordedMix.Create(false, null, null);
        }
        else
        {
            mixResult = PrerecordedMix.Create(true, uploadedFileName, azuraCastMediaId);
        }

        if (mixResult.IsSuccess)
            Prerecord = mixResult.Value;

        return mixResult.ToNoValue();
    }

    public DomainResult<NoValue> DeletePrerecordedMix()
    {
        if (!Prerecord.IsPrerecorded)
            return DomainResult.Err<NoValue>(new RuleError("No mix to delete"));

        var result = PrerecordedMix.Create(false, null, null);

        if (result.IsSuccess)
            Prerecord = result.Value;

        return result.ToNoValue();
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
                    .ComplexProperty(slot => slot.Prerecord);

        modelBuilder.Entity<HourlySlot>()
                    .ComplexProperty(slot => slot.TimeRange);

        modelBuilder.Entity<HourlySlot>()
                    .HasIndex(slot => slot.TimeRange.StartsAt)
                    .IsUnique();

        modelBuilder.Entity<HourlySlot>().HasOne(slot => slot.Performer).WithMany()
                    .HasForeignKey(slot => slot.PerformerId).ExcludeForeignKeyFromMigrations();
        modelBuilder.Entity<HourlySlot>().Navigation(slot => slot.Performer).AutoInclude();

        modelBuilder.Entity<HourlySlot>().ComplexProperty(slot => slot.Reminder);
    }
}