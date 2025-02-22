using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerRequestMapper : RequestMapper<PerformerRequest, Domain.Entities.Performer>
{
    public override Domain.Entities.Performer ToEntity(PerformerRequest r)
    {
        return new Domain.Entities.Performer
        {
            Id = Guid.NewGuid(),
            Name = r.Name.Trim(),
            Url = r.Url.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            LinkedUserIds = new()
        };
    }
}
