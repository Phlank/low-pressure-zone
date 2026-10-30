using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.CommunityAggregate.Events;

public record NameChanged(Guid CommunityId) : IEvent;