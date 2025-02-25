using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceResponseMapper : ResponseMapper<AudienceResponse, Domain.Entities.Audience>
{
    public override AudienceResponse FromEntity(Domain.Entities.Audience e)
    {
        return new AudienceResponse
        {
            Id = e.Id,
            Name = e.Name,
            Url = e.Url
        };
    }
}
