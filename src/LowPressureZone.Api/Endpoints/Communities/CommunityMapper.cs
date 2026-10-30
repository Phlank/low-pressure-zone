using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Scheduling.Domain.CommunityAggregate;

namespace LowPressureZone.Api.Endpoints.Communities;

[RegisterService<CommunityMapper>(LifeTime.Singleton)]
public sealed class CommunityMapper(CommunityPermissions permissions)
    : IResponseMapper
{
    public CommunityResponse FromEntity(Community community)
        => new()
        {
            Id = community.Id,
            Name = community.Name,
            SocialUrl = community.SocialUrl,
            IsPerformable = permissions.IsPerformanceAuthorized(community),
            IsOrganizable = permissions.IsOrganizingAuthorized(community),
            IsEditable = permissions.IsEditAuthorized(community),
            IsDeletable = permissions.IsDeleteAuthorized(community) && !community.IsDeleted
        };
}