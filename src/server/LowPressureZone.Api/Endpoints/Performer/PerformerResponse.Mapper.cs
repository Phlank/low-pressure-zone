using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Performer;

public sealed class PerformerResponseMapper : ResponseMapper<PerformerResponse, Domain.Entities.Performer>
{
    public override PerformerResponse FromEntity(Domain.Entities.Performer e)
    {
        return new PerformerResponse
        {
            Id = e.Id,
            Name = e.Name,
            Url = e.Url,
            CreatedDate = e.CreatedDate,
            ModifiedDate = e.LastModifiedDate
        };
    }
}
