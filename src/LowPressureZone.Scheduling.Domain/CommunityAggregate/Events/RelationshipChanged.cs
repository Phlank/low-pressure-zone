using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.CommunityAggregate.Events;

public record RelationshipChanged(Guid CommunityId, Guid UserId) : IEvent;