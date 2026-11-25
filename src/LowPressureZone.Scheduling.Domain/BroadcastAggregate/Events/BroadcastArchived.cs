using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.BroadcastAggregate.Events;

public record BroadcastArchived(Guid Id) : IEvent;