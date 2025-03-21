using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class CommunityMapper(IHttpContextAccessor contextAccessor, CommunityRules rules) : IRequestMapper, IResponseMapper
{
    public Community ToEntity(CommunityRequest request)
        => new Community
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Url = request.Url.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };

    public async Task UpdateEntityAsync(CommunityRequest request, Community community, CancellationToken ct = default)
    {
        var dataContext = contextAccessor.Resolve<DataContext>();
        community.Name = request.Name;
        community.Url = request.Url;
        if (dataContext.ChangeTracker.HasChanges())
        {
            community.LastModifiedDate = DateTime.UtcNow;
            await dataContext.SaveChangesAsync(ct);
        }
    }

    public CommunityResponse FromEntity(Community community)
        => new CommunityResponse
        {
            Id = community.Id,
            Name = community.Name,
            Url = community.Url,
            IsPerformable = rules.IsPerformanceAuthorized(community),
            IsOrganizable = rules.IsOrganizingAuthorized(community),
            IsEditable = rules.IsEditAuthorized(community),
            IsDeletable = rules.IsDeleteAuthorized(community) && !community.IsDeleted,
        };
}
