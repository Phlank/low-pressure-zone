using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerRequestMapper : RequestMapper<PerformerRequest, Domain.Entities.Performer>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public PerformerRequestMapper(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public override Domain.Entities.Performer ToEntity(PerformerRequest r)
    {
        var user = _contextAccessor.GetAuthenticatedUserOrDefault() ?? throw Exceptions.NoAuthorizedUserInToEntityMap;

        return new Domain.Entities.Performer
        {
            Id = Guid.NewGuid(),
            Name = r.Name.Trim(),
            Url = r.Url.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            LinkedUserIds = new() { user.GetIdOrDefault() }
        };
    }
}
