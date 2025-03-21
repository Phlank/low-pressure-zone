﻿using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformers(DataContext dataContext, PerformerRules rules) : EndpointWithoutRequest<IEnumerable<PerformerResponse>, PerformerMapper>
{
    public override void Configure() => Get("/performers");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var performers = await dataContext.Performers
                                          .AsNoTracking()
                                          .OrderBy(performer => performer.Name)
                                          .ToListAsync(ct);
        performers.RemoveAll(rules.IsHiddenFromApi);
        var responses = performers.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}
