using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Data;
using LowPressureZone.Domain.CommunityAggregate;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class CommunityMapper(IHttpContextAccessor contextAccessor, CommunityRules rules)
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