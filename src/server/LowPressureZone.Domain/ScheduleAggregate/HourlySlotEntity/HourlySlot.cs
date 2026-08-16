using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlotTimeRangeObject;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Rules;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;

public class HourlySlot : Entity, ITimeRange
{
    [MaxLength(128)] public string? Subtitle { get; private set; }
    public Guid PerformerId { get; private set; }
    public Performer Performer { get; private init; } = null!;
    public Guid ScheduleId { get; private init; }
    public Schedule Schedule { get; init; } = null!;
    public PrerecordedMix Mix { get; private set; }
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
        var mixResult = PrerecordedMix.Create(uploadedFileName, azuraCastMediaId);
        var timeRangeResult = HourlySlotTimeRange.Create(startsAt, duration);
        List<RuleError> errors =
        [
            .. Rule.Apply(new SubtitleLengthCannotExceed128Rule(subtitle)),
            .. mixResult.Errors,
            .. timeRangeResult.Errors
        ];

        if (errors.Count > 0)
            return DomainResult.Err<HourlySlot>(errors);

        return DomainResult.Ok(new HourlySlot
        {
            Subtitle = subtitle?.Trim(),
            PerformerId = performerId,
            ScheduleId = scheduleId,
            Mix = mixResult.Value,
            TimeRange = timeRangeResult.Value,
            Id = id ?? Guid.NewGuid()
        });
    }

    public DomainResult<NoValue> ChangeSubtitle(string? subtitle)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance, new SubtitleLengthCannotExceed128Rule(subtitle));
        if (result.IsSuccess)
            Subtitle = subtitle;
        return result;
    }

    public DomainResult<NoValue> ChangePerformer(Guid performerId)
    {
        PerformerId = performerId;
        return DomainResult.Ok();
    }

    public DomainResult<NoValue> ChangePrerecordedMix(string? uploadedFileName, int? azuraCastMediaId = null)
    {
        var mixResult = PrerecordedMix.Create(uploadedFileName, azuraCastMediaId);
        if (mixResult.IsSuccess)
            Mix = mixResult.Value;
        return mixResult.ToNoValue();
    }

    public DomainResult<NoValue> ChangeTime(DateTimeOffset startsAt, int duration)
    {
        var timeRangeResult = HourlySlotTimeRange.Create(startsAt, duration);
        if (timeRangeResult.IsSuccess)
            TimeRange = timeRangeResult.Value;
        return timeRangeResult.ToNoValue();
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HourlySlot>()
                    .ComplexProperty(slot => slot.Mix);

        modelBuilder.Entity<HourlySlot>()
                    .ComplexProperty(slot => slot.TimeRange);

        modelBuilder.Entity<HourlySlot>()
                    .HasIndex(slot => slot.TimeRange.StartsAt)
                    .IsUnique();

        modelBuilder.Entity<HourlySlot>()
                    .HasOne(slot => slot.Performer)
                    .WithMany()
                    .HasForeignKey(slot => slot.PerformerId)
                    .ExcludeForeignKeyFromMigrations();
    }
}