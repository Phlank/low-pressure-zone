using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Test.Tests;

public static class AppUserClaimsTransformationTestsData
{
    private static readonly Guid CommunityId = Guid.NewGuid();
    public static readonly Guid OrganizerUserId = Guid.NewGuid();
    public static readonly Guid PerformerUserId = Guid.NewGuid();
    public static readonly Guid OrganizerPerformerUserId = Guid.NewGuid();

    public static readonly List<Community> Communities =
    [
        new()
        {
            Name = "AppUserClaimsTransformation Test Community",
            Url = "https://testcommunity.com",
            Relationships =
            {
                new CommunityRelationship
                {
                    CommunityId = CommunityId,
                    UserId = OrganizerUserId,
                    IsOrganizer = true,
                    IsPerformer = false
                },
                new CommunityRelationship
                {
                    CommunityId = CommunityId,
                    UserId = PerformerUserId,
                    IsOrganizer = false,
                    IsPerformer = true
                },
                new CommunityRelationship
                {
                    CommunityId = CommunityId,
                    UserId = OrganizerPerformerUserId,
                    IsOrganizer = true,
                    IsPerformer = true
                }
            }
        }
    ];
}