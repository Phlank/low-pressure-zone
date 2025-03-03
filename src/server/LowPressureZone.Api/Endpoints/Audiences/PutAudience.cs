using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class PutAudience : Endpoint<AudienceRequest, EmptyResponse, AudienceMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/audiences/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
        AllowAnonymous();
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
            ThrowError(new ValidationFailure(nameof(id), Errors.Unique));
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
