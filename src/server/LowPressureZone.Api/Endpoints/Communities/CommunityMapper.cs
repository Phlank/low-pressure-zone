using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.CommunityAggregate;

namespace LowPressureZone.Api.Endpoints.Communities;

[RegisterService<CommunityMapper>(LifeTime.Singleton)]
public sealed class CommunityMapper(CommunityRules rules)
    : IResponseMapper
{
    public CommunityResponse FromEntity(Community community)
        => new()
        {
            Id = community.Id,
            Name = community.Name,
            SocialUrl = community.SocialUrl,
            IsPerformable = rules.IsPerformanceAuthorized(community),
            IsOrganizable = rules.IsOrganizingAuthorized(community),
            IsEditable = rules.IsEditAuthorized(community),
            IsDeletable = rules.IsDeleteAuthorized(community) && !community.IsDeleted
        };
}