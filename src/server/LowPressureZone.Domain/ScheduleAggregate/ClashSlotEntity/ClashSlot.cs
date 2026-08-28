using System.ComponentModel.DataAnnotations.Schema;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.ClashTimeRangeObject;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity.Rules;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.Rules;
using LowPressureZone.Domain.ScheduleAggregate.Rules;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;

[Table("ClashSlots")]
public class ClashSlot : Entity, ITimeRange
{
    public Guid ScheduleId { get; private init; }
    public Schedule Schedule { get; private init; } = null!;
    public Guid PerformerOneId { get; private set; }
    public Performer PerformerOne { get; private init; } = null!;
    public Guid PerformerTwoId { get; private set; }
    public Performer PerformerTwo { get; private init; } = null!;

    public List<string> Rounds
    {
        get;
        private set => field =
        [
            .. value.Where(item => !string.IsNullOrWhiteSpace(item))
                    .Select(item => item.Trim())
        ];
    } = [];

    public ClashTimeRange TimeRange { get; private set; }

    public DateTimeOffset StartsAt => TimeRange.StartsAt;
    public DateTimeOffset EndsAt => TimeRange.EndsAt;
    public TimeSpan TimeSpan => TimeRange.TimeSpan;

    // EF Core constructor
    private ClashSlot()
    {
    }

    private ClashSlot(Guid scheduleId,
                      Guid performerOneId,
                      Guid performerTwoId,
                      List<string> rounds,
                      ClashTimeRange timeRange,
                      Guid? id = null)
    {
        ScheduleId = scheduleId;
        PerformerOneId = performerOneId;
        PerformerTwoId = performerTwoId;
        Rounds = rounds;
        TimeRange = timeRange;
        Id = id ?? Guid.NewGuid();
    }

    public static DomainResult<ClashSlot> Create(Guid scheduleId,
                                                 Guid performerOneId,
                                                 Guid performerTwoId,
                                                 List<string> rounds,
                                                 DateTimeOffset startsAt,
                                                 int duration)
    {
        var timeRangeResult = ClashTimeRange.Create(startsAt, duration);
        List<RuleError> errors =
        [
            .. Rule.Apply(new RoundsMustBeProvidedRule(rounds),
                                       new PerformersMustBeDifferentRule(performerOneId, performerTwoId)),
            .. timeRangeResult.Errors
        ];

        if (errors.Count > 0)
        {
            return DomainResult.Err<ClashSlot>(errors);
        }

        return DomainResult.Ok(new ClashSlot(scheduleId,
                                             performerOneId,
                                             performerTwoId,
                                             rounds,
                                             timeRangeResult.Value));
    }

    public DomainResult<NoValue> ChangePerformers(Guid performerOneId, Guid performerTwoId)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                                       new PerformersMustBeDifferentRule(performerOneId, performerTwoId));
        
        if (result.IsSuccess)
        {
            PerformerOneId = performerOneId;
            PerformerTwoId = performerTwoId;
        }

        return result;
    }

    public DomainResult<NoValue> ChangeRounds(List<string> rounds)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                                       new RoundsMustBeProvidedRule(rounds));
        if (result.IsSuccess)
            Rounds = rounds;
        
        return result;
    }

    public DomainResult<NoValue> ChangeTime(DateTimeOffset startsAt, int duration)
    {
        var additionalErrors = Rule.Apply(new CannotChangeSlotAfterItEndsRule(this));
        var result = ClashTimeRange.Create(startsAt, duration)
                                   .WithAdditionalErrors(additionalErrors);
        if (result.IsSuccess)
        {
            TimeRange = result.Value;
            return DomainResult.Ok();
        }

        return result.ToNoValue();
    }

    public DomainResult<NoValue> Delete()
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance, new CannotChangeSlotAfterItEndsRule(this));
        if (result.IsSuccess)
        {
            Schedule.ClashSlots.Remove(this);
        }

        return result;
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClashSlot>()
                    .ComplexProperty(clash => clash.TimeRange);

        modelBuilder.Entity<ClashSlot>()
                    .HasIndex(slot => slot.TimeRange.StartsAt)
                    .IsUnique();

        modelBuilder.Entity<ClashSlot>().HasOne(slot => slot.PerformerOne).WithMany()
                    .HasForeignKey(slot => slot.PerformerOneId).ExcludeForeignKeyFromMigrations();
        modelBuilder.Entity<ClashSlot>().Navigation(slot => slot.PerformerOne).AutoInclude();

        modelBuilder.Entity<ClashSlot>().HasOne(slot => slot.PerformerTwo).WithMany()
                    .HasForeignKey(slot => slot.PerformerTwoId).ExcludeForeignKeyFromMigrations();
        modelBuilder.Entity<ClashSlot>().Navigation(slot => slot.PerformerTwo).AutoInclude();
    }
}