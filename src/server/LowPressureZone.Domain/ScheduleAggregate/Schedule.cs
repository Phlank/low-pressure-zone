using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain.CommunityAggregate;
using LowPressureZone.Domain.ScheduleAggregate.AllowedScheduleSlotTypesObject;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Domain.ScheduleAggregate.Rules;
using LowPressureZone.Domain.ScheduleAggregate.ScheduleTimeRangeObject;
using Microsoft.EntityFrameworkCore;
using NameIsRequiredRule = LowPressureZone.Domain.ScheduleAggregate.Rules.NameIsRequiredRule;

namespace LowPressureZone.Domain.ScheduleAggregate;

public class Schedule : Entity
{
    [MaxLength(256)]
    public string Name
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    [MaxLength(16384)]
    public string Description
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    public ScheduleTimeRange TimeRange { get; private set; }
    public AllowedScheduleSlotTypes AllowedSlotTypes { get; private set; }
    public List<ClashSlot> ClashSlots { get; private init; } = [];
    public List<HourlySlot> HourlySlots { get; private init; } = [];
    public Guid CommunityId { get; set; }
    public Community Community { get; set; } = null!;
    public bool IsVisibleToPublic { get; private set; }

    public List<ITimeRange> SlotTimeRanges =>
    [
        .. ClashSlots.Select(clash => clash.TimeRange as ITimeRange),
        .. HourlySlots.Select(slot => slot.TimeRange as ITimeRange)
    ];

    // EF Core constructor
    private Schedule()
    {
    }

    private Schedule(string name,
                     string description,
                     Guid communityId,
                     ScheduleTimeRange timeRange,
                     AllowedScheduleSlotTypes allowedSlotTypes,
                     bool isVisibleToPublic)
    {
        Name = name;
        Description = description;
        CommunityId = communityId;
        TimeRange = timeRange;
        AllowedSlotTypes = allowedSlotTypes;
        IsVisibleToPublic = isVisibleToPublic;
    }

    public static DomainResult<Schedule> Create(string name,
                                                string description,
                                                Guid communityId,
                                                DateTimeOffset startsAt,
                                                DateTimeOffset endsAt,
                                                bool isHourlyAllowed,
                                                bool isClashAllowed,
                                                bool isVisibleToPublic)
    {
        var timeRangeResult = ScheduleTimeRange.Create(startsAt, endsAt);
        var allowedSlotTypesResult = AllowedScheduleSlotTypes.Create(isHourlyAllowed, isClashAllowed);

        List<RuleError> errors =
        [
            .. Rule.Apply(new NameIsRequiredRule(name),
                          new NameLengthCannotExceed256Rule(name),
                          new DescriptionLengthCannotExceed16384Rule(description)),
            .. timeRangeResult.Errors,
            .. allowedSlotTypesResult.Errors
        ];

        if (errors.Count > 0) return DomainResult.Err<Schedule>(errors);

        return DomainResult.Ok(new Schedule(name, description, communityId, timeRangeResult.Value,
                                            allowedSlotTypesResult.Value,
                                            isVisibleToPublic));
    }

    public DomainResult<NoValue> AddHourlySlot(HourlySlot slot)
    {
        var errors = Rule.Apply(new SlotMustBeWithinScheduleTimeRange(TimeRange, slot),
                                new SlotsCannotHaveOverlappingTimeRangesRule(SlotTimeRanges, slot));
        if (errors.Count > 0) return DomainResult.Err<NoValue>(errors);

        HourlySlots.Add(slot);
        return DomainResult.Ok();
    }

    public DomainResult<NoValue> AddClashSlot(ClashSlot slot)
    {
        var errors = Rule.Apply(new SlotMustBeWithinScheduleTimeRange(TimeRange, slot),
                                new SlotsCannotHaveOverlappingTimeRangesRule(SlotTimeRanges, slot));
        if (errors.Count > 0) return DomainResult.Err<NoValue>(errors);

        ClashSlots.Add(slot);
        return DomainResult.Ok();
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Schedule>()
                    .ComplexProperty(schedule => schedule.TimeRange);

        modelBuilder.Entity<Schedule>()
                    .ComplexProperty(schedule => schedule.AllowedSlotTypes);

        modelBuilder.Entity<Schedule>()
                    .HasIndex(schedule => schedule.TimeRange.StartsAt)
                    .IsUnique();
    }
}