using FastEndpoints;

namespace LowPressureZone.Domain.CommunityAggregate.Events;

public record NameChanged(Guid CommunityId) : IEvent;