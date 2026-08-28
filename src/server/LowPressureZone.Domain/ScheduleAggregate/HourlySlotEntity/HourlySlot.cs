using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Extensions;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Rules;
using LowPressureZone.Domain.ScheduleAggregate.Rules;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;

[Table("HourlySlots")]
public class HourlySlot : Entity, ITimeRange
{
    [MaxLength(128)]
    public string? Subtitle { get; private set; }

    public Guid PerformerId { get; private set; }
    public Performer Performer { get; private init; } = null!;
    public Guid ScheduleId { get; private init; }
    public Schedule Schedule { get; init; } = null!;
    public PrerecordedMix Prerecord { get; private set; }
    public HourlySlotTimeRange TimeRange { get; private set; }
    public DateTimeOffset StartsAt => TimeRange.StartsAt;
    public DateTimeOffset EndsAt => TimeRange.EndsAt;
    public TimeSpan TimeSpan => TimeRange.TimeSpan;

    public static DomainResult<HourlySlot> Create(
        Guid scheduleId,
        Guid performerId,
        DateTimeOffset startsAt,
        int duration,
        string? subtitle = null,
        string? uploadedFileName = null,
        int? azuraCastMediaId = null,
        Guid? id = null)
    {
        var prerecordedMixResult = PrerecordedMix.Create(uploadedFileName is not null,
                                                         uploadedFileName,
                                                         azuraCastMediaId);
        var timeRangeResult = HourlySlotTimeRange.Create(startsAt, duration);
        List<RuleError> errors =
        [
            .. Rule.Apply(new SubtitleLengthCannotExceed128Rule(subtitle)),
            .. prerecordedMixResult.Errors,
            .. timeRangeResult.Errors
        ];

        if (errors.Count > 0)
            return DomainResult.Err<HourlySlot>(errors);

        return DomainResult.Ok(new HourlySlot
        {
            Subtitle = subtitle?.Trim(),
            PerformerId = performerId,
            ScheduleId = scheduleId,
            Prerecord = prerecordedMixResult.Value,
            TimeRange = timeRangeResult.Value,
            Id = id ?? Guid.NewGuid()
        });
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
        var mixResult = PrerecordedMix.Create(true, uploadedFileName, azuraCastMediaId);

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
        var timeRangeResult = HourlySlotTimeRange.Create(startsAt, duration)
                                                 .WithAdditionalRules(new CannotChangeSlotAfterItEndsRule(this));
        
        if (timeRangeResult.IsSuccess)
            TimeRange = timeRangeResult.Value;

        return timeRangeResult.ToNoValue();
    }

    public DomainResult<NoValue> Delete()
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                                       new CannotChangeSlotAfterItEndsRule(this));
        if (result.IsSuccess)
        {
            Schedule.HourlySlots.Remove(this);
        }

        return result;
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}