using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class CommunityMapper(CommunityRules rules) : Mapper<CommunityRequest, CommunityResponse, Community>,
    IRequestMapper, IResponseMapper
{
    public override Community ToEntity(CommunityRequest request)
    {
        return new Community
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Url = request.Url.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };
    }

    public override Task<Community> ToEntityAsync(CommunityRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(ToEntity(request));
    }

    public override async Task<Community> UpdateEntityAsync(CommunityRequest request, Community community,
        CancellationToken ct = default)
    {
        var dataContext = Resolve<DataContext>();
        community.Name = request.Name;
        community.Url = request.Url;
        if (dataContext.ChangeTracker.HasChanges())
        {
            community.LastModifiedDate = DateTime.UtcNow;
            await dataContext.SaveChangesAsync(ct);
        }

        return community;
    }

    public override CommunityResponse FromEntity(Community community)
    {
        return new CommunityResponse
        {
            Id = community.Id,
            Name = community.Name,
            Url = community.Url,
            IsRelated = rules.IsRelated(community),
            IsEditable = rules.IsEditAuthorized(community),
            IsDeletable = rules.IsDeleteAuthorized(community) && !community.IsDeleted,
            IsLinkableToSchedule = rules.IsScheduleLinkAuthorized(community)
        };
    }

    public override Task<CommunityResponse> FromEntityAsync(Community community, CancellationToken ct = default)
    {
        return Task.FromResult(FromEntity(community));
    }
}
