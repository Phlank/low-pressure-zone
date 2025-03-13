using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceMapper(AudienceRules rules) : Mapper<AudienceRequest, AudienceResponse, Audience>, IRequestMapper, IResponseMapper
{
    public override Audience ToEntity(AudienceRequest req)
    {
        return new Audience
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Url = req.Url.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };
    }

    public override Task<Audience> ToEntityAsync(AudienceRequest r, CancellationToken ct = default)
        => Task.FromResult(ToEntity(r));

    public override async Task<Audience> UpdateEntityAsync(AudienceRequest req, Audience audience, CancellationToken ct = default)
    {
        var dataContext = Resolve<DataContext>();
        audience.Name = req.Name;
        audience.Url = req.Url;
        if (dataContext.ChangeTracker.HasChanges())
        {
            audience.LastModifiedDate = DateTime.UtcNow;
            await dataContext.SaveChangesAsync(ct);
        }
        return audience;
    }

    public override AudienceResponse FromEntity(Audience audience)
    {
        return new AudienceResponse
        {
            Id = audience.Id,
            Name = audience.Name,
            Url = audience.Url,
            IsRelated = rules.IsRelated(audience),
            IsEditable = rules.IsEditAuthorized(audience),
            IsDeletable = rules.IsDeleteAuthorized(audience) && !audience.IsDeleted,
            IsLinkableToSchedule = rules.IsScheduleLinkAuthorized(audience)
        };
    }

    public override Task<AudienceResponse> FromEntityAsync(Audience audience, CancellationToken ct = default)
        => Task.FromResult(FromEntity(audience));
}
