using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class PutAudience : Endpoint<AudienceRequest, EmptyResponse, AudienceRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/audiences/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
    }

    public override async Task HandleAsync(AudienceRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var existingAudience = DataContext.Audiences.Find(id);
        if (existingAudience == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var isNameInUse = DataContext.Audiences.Any(a => a.Name == req.Name && a.Id != id);
        if (isNameInUse)
        {
            ThrowError(new ValidationFailure(nameof(id), "Audience name already in use."));
        }

        if (existingAudience.Name != req.Name || existingAudience.Url != req.Url)
        {
            existingAudience.Name = req.Name;
            existingAudience.Url = req.Url;
            existingAudience.LastModifiedDate = DateTime.UtcNow;
            DataContext.SaveChanges();
        }

        await SendNoContentAsync();
    }
}
