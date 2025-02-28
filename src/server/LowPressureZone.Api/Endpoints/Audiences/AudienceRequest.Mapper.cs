﻿using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceRequestMapper : RequestMapper<AudienceRequest, Domain.Entities.Audience>
{
    public override Domain.Entities.Audience ToEntity(AudienceRequest r)
    {
        return new Domain.Entities.Audience
        {
            Id = Guid.NewGuid(),
            Name = r.Name.Trim(),
            Url = r.Url.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };
    }
}
