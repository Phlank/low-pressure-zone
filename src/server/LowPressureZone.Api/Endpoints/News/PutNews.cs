using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.News;

public class PutNews(DataContext dataContext) : Endpoint<NewsRequest>
{
    public override void Configure()
    {
        Put("/news/{id:guid}");
        Roles(RoleNames.Admin);
        Description(builder => builder.WithTags("News")
                                      .Produces(204)
                                      .Produces<ValidationProblemDetails>(400)
                                      .Produces(404));
    }

    public override async Task HandleAsync(NewsRequest request, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var news = await dataContext.News
                                                  .FirstOrDefaultAsync(news => news.Id == id, ct);
        if (news == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var result = news.Edit(request.Title, request.Content);
        this.ThrowIfDomainError(result);
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}