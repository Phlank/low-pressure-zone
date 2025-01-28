using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audience;

public sealed class PutAudience : Endpoint<PutAudienceRequest, EmptyResponse, AudienceDtoMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/audience");
        Description(b => b.Produces(200)
                          .Produces(201)
                          .ProducesProblem(400)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(PutAudienceRequest req, CancellationToken ct)
    {
        var nameExists = DataContext.Audiences.Any(aud => aud.Name == req.Name && aud.Id != req.Id);
        if (nameExists)
        {
            ThrowError(new ValidationFailure(nameof(req.Name), "Duplicate audience names not allowed.", req.Name));
        }

        if (req.Id is null)
        {
            DataContext.Audiences.Add(Map.ToEntity(req));
            DataContext.SaveChanges();
            await SendCreatedAtAsync<GetAudience>(null, Response);
            return;
        }

        var audience = DataContext.Audiences.FirstOrDefault(aud => aud.Id == req.Id);
        if (audience == null)
        {
            await SendNotFoundAsync();
            return;
        }

        if (audience.Name != req.Name || audience.Url != req.Url)
        {
            audience.LastModifiedDate = DateTime.UtcNow;
            audience.Name = req.Name;
            audience.Url = req.Url;
            DataContext.SaveChanges();
        }
        await SendOkAsync();
    }
}
