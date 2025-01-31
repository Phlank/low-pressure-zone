using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Audience;

public sealed class PostAudience : Endpoint<AudienceRequest, EmptyResponse, AudienceRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Post("/audience");
        Description(b => b.Produces(201));
    }

    public override async Task HandleAsync(AudienceRequest req, CancellationToken ct)
    {
        var isNameInUse = DataContext.Audiences.Any(a => a.Name == req.Name);
        if (isNameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Name), "Audience name already in use.", req.Name));
        }

        var entity = Map.ToEntity(req);
        DataContext.Audiences.Add(entity);
        DataContext.SaveChanges();
        await SendCreatedAtAsync<GetAudience>(new { entity.Id }, Response);
    }
}
