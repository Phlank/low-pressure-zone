using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;
using static FastEndpoints.Ep;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceMapper(IHttpContextAccessor accessor, AudienceRules rules) : Mapper<AudienceRequest, AudienceResponse, Audience>, IRequestMapper, IResponseMapper
{
    public override Audience ToEntity(AudienceRequest req)
    {
        var user = accessor.GetAuthenticatedUserOrDefault() ?? throw Exceptions.NoAuthorizedUserForToEntityMap;
        return new Audience
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Url = req.Url.Trim(),
            LinkedUserIds = new() { user.GetIdOrDefault() },
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
            IsEditable = rules.IsEditAuthorized(audience),
            IsDeletable = rules.IsDeleteAuthorized(audience) && !audience.IsDeleted,
            IsLinkableToSchedule = rules.IsScheduleLinkAuthorized(audience)
        };
    }

    public override Task<AudienceResponse> FromEntityAsync(Audience audience, CancellationToken ct = default)
        => Task.FromResult(FromEntity(audience));
}
