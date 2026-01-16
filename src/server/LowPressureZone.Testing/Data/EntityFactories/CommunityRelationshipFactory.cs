using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Testing.Data.EntityFactories;

public static class CommunityRelationshipFactory
{
    public static CommunityRelationship Create(
        Guid? communityId = null,
        Guid? userId = null,
        bool isOrganizer = false,
        bool isPerformer = false) =>
        new()
        {
            CommunityId = communityId ?? Guid.Empty,
            UserId = userId ?? Guid.Empty,
            IsOrganizer = isOrganizer,
            IsPerformer = isPerformer
        };
}