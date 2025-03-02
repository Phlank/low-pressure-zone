using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceMapper : Mapper<AudienceRequest, AudienceResponse, Audience>, IRequestMapper, IResponseMapper
{
    private readonly IHttpContextAccessor _accessor;
    private readonly AudienceRules _rules;

    public AudienceMapper(IHttpContextAccessor accessor, AudienceRules rules)
    {
        _accessor = accessor;
        _rules = rules;
    }

    public override Audience ToEntity(AudienceRequest r)
    {
        var user = _accessor.GetAuthenticatedUserOrDefault() ?? throw Exceptions.NoAuthorizedUserInToEntityMap;
        return new Audience
        {
            Id = Guid.NewGuid(),
            Name = r.Name.Trim(),
            Url = r.Url.Trim(),
            LinkedUserIds = new() { user.GetIdOrDefault() },
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };
    }

    public override AudienceResponse FromEntity(Audience e)
    {
        return new AudienceResponse
        {
            Id = e.Id,
            Name = e.Name,
            Url = e.Url,
            IsEditable = _rules.IsEditAuthorized(e),
            IsDeletable = _rules.IsDeleteAuthorized(e),
            IsLinkableToSchedule = _rules.IsScheduleLinkAuthorized(e)
        };
    }
}
