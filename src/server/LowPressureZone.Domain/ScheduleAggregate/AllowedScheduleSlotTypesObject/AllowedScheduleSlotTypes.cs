using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.ScheduleAggregate.AllowedScheduleSlotTypesObject.Rules;

namespace LowPressureZone.Domain.ScheduleAggregate.AllowedScheduleSlotTypesObject;

public readonly record struct AllowedScheduleSlotTypes
{
    public bool IsHourlyAllowed { get; private init; }
    public bool IsClashAllowed { get; private init; }

    public static DomainResult<AllowedScheduleSlotTypes> Create(bool isHourlyAllowed, bool isClashAllowed) =>
        Rule.ApplyIntoResult(new AllowedScheduleSlotTypes
                                          {
                                              IsHourlyAllowed = isHourlyAllowed,
                                              IsClashAllowed = isClashAllowed
                                          },
                                          new AtLeastOneSlotTypeMustBeAllowedRule(isHourlyAllowed, isClashAllowed));
}