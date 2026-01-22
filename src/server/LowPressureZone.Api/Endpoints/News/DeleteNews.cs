using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.News;

public class DeleteNews(DataContext dataContext) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/news/{id:guid}");
        Roles(RoleNames.Admin);
        Description(builder => builder.WithTags("News")
                                      .Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var countDeleted = await dataContext.News
                                            .Where(news => news.Id == id)
                                            .ExecuteDeleteAsync(ct);
        if (countDeleted == 0)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}