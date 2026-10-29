using FastEndpoints;

namespace LowPressureZone.Domain.CommunityAggregate.Events;

public record RelationshipChanged(Guid CommunityId, Guid UserId) : IEvent;