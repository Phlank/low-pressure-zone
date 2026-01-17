using LowPressureZone.Domain.Entities;
using LowPressureZone.Testing.Data.EntityFactories;

namespace LowPressureZone.Testing.Tests.Authentication;

public static class AppUserClaimsTransformationTestsData
{
    private static readonly Guid CommunityId = Guid.NewGuid();
    public static readonly Guid OrganizerUserId = Guid.NewGuid();
    public static readonly Guid PerformerUserId = Guid.NewGuid();
    public static readonly Guid OrganizerPerformerUserId = Guid.NewGuid();

    public static readonly List<Community> Communities =
    [
        CommunityFactory.Create(id: CommunityId,
                                name: "AppUserClaimsTransformation Test Community",
                                url: "https://testcommunity.com",
                                relationships:
                                [
                                    CommunityRelationshipFactory.Create(communityId: CommunityId,
                                                                        userId: OrganizerUserId, 
                                                                        isOrganizer: true),
                                    CommunityRelationshipFactory.Create(communityId: CommunityId,
                                                                        userId: PerformerUserId, 
                                                                        isPerformer: true),
                                    CommunityRelationshipFactory.Create(communityId: CommunityId,
                                                                        userId: OrganizerPerformerUserId,
                                                                        isOrganizer: true, 
                                                                        isPerformer: true),
                                ])
    ];
}