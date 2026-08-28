using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Core;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

public class ClashSlotResponse : ITimeRange
{
    public required Guid Id { get; set; }
    public required Guid ScheduleId { get; set; }
    public required PerformerResponse PerformerOne { get; set; }
    public required PerformerResponse PerformerTwo { get; set; }
    public required List<string> Rounds { get; set; } = [];
    public required DateTimeOffset StartsAt { get; set; }
    public required int Duration { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
}