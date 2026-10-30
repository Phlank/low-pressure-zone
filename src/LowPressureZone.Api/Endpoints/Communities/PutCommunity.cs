using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities.Rules;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class PutCommunity(
    DataContext dataContext,
    NameMustBeUniqueRule nameMustBeUniqueRule,
    SocialUrlMustBeUniqueRule socialUrlMustBeUniqueRule) : Endpoint<CommunityRequest>
{
    public override void Configure()
    {
        Put("/communities/{id}");
        Description(builder => builder.Produces(404));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CommunityRequest request, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var community = await dataContext.Communities.FirstOrDefaultAsync(community => community.Id == id, ct);

        if (community is null || community.IsDeleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var result = DomainResult.Compose(community.Rename(request.Name),
                                          community.ChangeSocialUrl(request.SocialUrl));
        this.ThrowIfDomainErrors(result);
        this.ThrowIfRuleErrors(await Rule.ApplyAsync(community, [nameMustBeUniqueRule, socialUrlMustBeUniqueRule], ct));

        await this.PublishEventsAsync(result.Events);
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}