using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerResponseMapper : ResponseMapper<PerformerResponse, Domain.Entities.Performer>
{
    public override PerformerResponse FromEntity(Domain.Entities.Performer e)
    {
        return new PerformerResponse
        {
            Id = e.Id,
            Name = e.Name,
            Url = e.Url,
            CanDelete = false,
            IsLinked = false
        };
    }
}
