using FastEndpoints;

namespace LowPressureZone.Domain.CommunityAggregate.Events;

public record SocialUrlChanged(Guid CommunityId) : IEvent;