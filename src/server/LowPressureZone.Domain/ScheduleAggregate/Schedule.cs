using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
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

    public List<Slot> Slots =>
    [
        .. ClashSlots.Select(slot => new Slot(slot)),
        .. HourlySlots.Select(slot => new Slot(slot))
    ];

    public Guid CommunityId { get; init; }
    public Community Community { get; init; } = null!;
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

        if (errors.Count > 0)
            return DomainResult.Err<Schedule>(errors);

        return DomainResult.Ok(new Schedule(name,
                                            description,
                                            communityId,
                                            timeRangeResult.Value,
                                            allowedSlotTypesResult.Value,
                                            isVisibleToPublic));
    }

    public DomainResult<NoValue> ChangeName(string name)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new NameIsRequiredRule(name),
                                          new NameLengthCannotExceed256Rule(name));

        if (result.IsSuccess)
            Name = name;

        return result;
    }

    public DomainResult<NoValue> ChangeDescription(string description)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new DescriptionLengthCannotExceed16384Rule(description));

        if (result.IsSuccess)
            Description = description;

        return result;
    }

    public DomainResult<NoValue> ChangeAllowedSlotTypes(bool isHourlyAllowed, bool isClashAllowed)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new NoHourlySlotsWhenNotAllowed(HourlySlots, isHourlyAllowed),
                                          new NoClashSlotsWhenNotAllowed(ClashSlots, isClashAllowed));

        if (result.IsSuccess)
        {
            AllowedSlotTypes = AllowedScheduleSlotTypes.Create(isHourlyAllowed, isClashAllowed).Value;
        }

        return result;
    }

    public DomainResult<NoValue> ChangeVisibility(bool isVisibleToPublic)
    {
        IsVisibleToPublic = isVisibleToPublic;
        return DomainResult.Ok();
    }

    public DomainResult<NoValue> ChangeTime(DateTimeOffset startsAt, DateTimeOffset endsAt)
    {
        var timeRangeResult = ScheduleTimeRange.Create(startsAt, endsAt);
        if (timeRangeResult.IsError)
            return timeRangeResult.ToNoValue();

        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new SlotsMustBeWithinScheduleTimeRange(timeRangeResult.Value,
                                                                                 [.. HourlySlots, .. ClashSlots]));
        if (result.IsSuccess)
            TimeRange = timeRangeResult.Value;

        return result;
    }

    public DomainResult<NoValue> AddHourlySlot(HourlySlot slot)
    {
        var errors = Rule.Apply(new SlotsMustBeWithinScheduleTimeRange(TimeRange,
                                                                       [slot, .. HourlySlots, .. ClashSlots]),
                                new SlotsCannotHaveOverlappingTimeRangesRule(SlotTimeRanges, slot));
        if (errors.Count > 0) return DomainResult.Err<NoValue>(errors);

        HourlySlots.Add(slot);
        return DomainResult.Ok();
    }

    public DomainResult<NoValue> AddClashSlot(ClashSlot slot)
    {
        var errors = Rule.Apply(new SlotsMustBeWithinScheduleTimeRange(TimeRange,
                                                                       [slot, .. HourlySlots, .. ClashSlots]),
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

        modelBuilder.Entity<Schedule>().Navigation(schedule => schedule.Community).AutoInclude();
        modelBuilder.Entity<Schedule>().HasOne(schedule => schedule.Community).WithMany()
                    .HasForeignKey(schedule => schedule.CommunityId).ExcludeForeignKeyFromMigrations();

        modelBuilder.Entity<Schedule>().HasMany(schedule => schedule.HourlySlots).WithOne(slot => slot.Schedule)
                    .HasForeignKey(slot => slot.ScheduleId).ExcludeForeignKeyFromMigrations();
        modelBuilder.Entity<Schedule>().Navigation(schedule => schedule.HourlySlots).AutoInclude();

        modelBuilder.Entity<Schedule>().HasMany(schedule => schedule.ClashSlots).WithOne(slot => slot.Schedule)
                    .HasForeignKey(slot => slot.ScheduleId).ExcludeForeignKeyFromMigrations();
        modelBuilder.Entity<Schedule>().Navigation(schedule => schedule.ClashSlots).AutoInclude();
    }
}