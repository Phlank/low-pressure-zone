using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Audience;

public class AudienceDtoMapper : Mapper<PutAudienceRequest, GetAudienceResponse, Domain.Entities.Audience>
{
    public override Domain.Entities.Audience ToEntity(PutAudienceRequest r)
    {
        return new Domain.Entities.Audience
        {
            Name = r.Name,
            Url = r.Url,
        };
    }

    public override GetAudienceResponse FromEntity(Domain.Entities.Audience e)
    {
        return new GetAudienceResponse
        {
            Id = e.Id,
            Name = e.Name,
            Url = e.Url,
            CreatedDate = e.CreatedDate,
            ModifiedDate = e.LastModifiedDate
        };
    }
}