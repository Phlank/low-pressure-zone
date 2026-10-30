using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.CommunityAggregate.Events;

public record NameChanged(Guid CommunityId) : IEvent;