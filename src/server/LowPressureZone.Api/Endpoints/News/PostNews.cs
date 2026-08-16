using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.News;

public class PostNews(DataContext dataContext) : Endpoint<NewsRequest>
{
    public override void Configure()
    {
        Post("/news");
        Roles(RoleNames.Admin);
        Description(builder => builder.WithTags("News")
                                      .Produces(201));
    }

    public override async Task HandleAsync(NewsRequest req, CancellationToken ct)
    {
        var result = Domain.NewsAggregate.News.Create(req.Title, req.Content);
        this.ThrowIfDomainError(result);

        dataContext.Add(result.Value);
        await dataContext.SaveChangesAsync(ct);

        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetNewsById>(new
        {
            result.Value.Id
        }, Response, cancellation: ct);
    }
}