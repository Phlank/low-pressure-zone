﻿using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Performer;

public sealed class PerformerRequestMapper : RequestMapper<PerformerRequest, Domain.Entities.Performer>
{
    public override Domain.Entities.Performer ToEntity(PerformerRequest r)
    {
        return new Domain.Entities.Performer
        {
            Id = Guid.NewGuid(),
            Name = r.Name,
            Url = r.Url,
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
        };
    }
}
