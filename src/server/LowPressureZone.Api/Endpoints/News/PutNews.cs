using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.News;

public class PutNews(DataContext dataContext) : EndpointWithMapper<NewsRequest, NewsMapper>
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
        var persistentNewsItem = await dataContext.News
                                                  .FirstOrDefaultAsync(news => news.Id == id, ct);
        if (persistentNewsItem == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var mapped = Map.ToEntity(request);

        persistentNewsItem.Title = mapped.Title;
        persistentNewsItem.Body = mapped.Body;
        persistentNewsItem.LastModifiedDate = mapped.LastModifiedDate;
        await dataContext.SaveChangesAsync(ct);
    }
}