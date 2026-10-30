using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.CommunityAggregate.Events;

public record SocialUrlChanged(Guid CommunityId) : IEvent;