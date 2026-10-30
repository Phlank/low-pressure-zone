using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities.Rules;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Domain.CommunityAggregate;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class PostCommunity(DataContext dataContext, NameMustBeUniqueRule nameMustBeUniqueRule)
    : Endpoint<CommunityRequest>
{
    public override void Configure()
    {
        Post("/communities");
        Description(b => b.Produces(201));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CommunityRequest req, CancellationToken ct)
    {
        var result = Community.Create(req.Name, req.SocialUrl);
        this.ThrowIfDomainErrors(result);
        
        var asyncErrors = await Rule.ApplyAsync(result.Value, [nameMustBeUniqueRule], ct);
        this.ThrowIfRuleErrors(asyncErrors);

        dataContext.Communities.Add(result.Value);
        await dataContext.SaveChangesAsync(ct);
        
        await this.PublishEventsAsync(result.Events);
        
        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetCommunityById>(new
                                                    {
                                                        id = result.Value.Id
                                                    },
                                                    Response,
                                                    cancellation: ct);
    }
}