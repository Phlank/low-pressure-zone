using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class PostAudience : EndpointWithMapper<AudienceRequest, AudienceMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Post("/audiences");
        Description(b => b.Produces(201));
        AllowAnonymous();
    }

    public override async Task HandleAsync(AudienceRequest req, CancellationToken ct)
    {
        var isNameInUse = DataContext.Audiences.Any(a => a.Name == req.Name);
        if (isNameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Name), Errors.Unique, req.Name));
        }

        var entity = Map.ToEntity(req);
        DataContext.Audiences.Add(entity);
        DataContext.SaveChanges();
        await SendCreatedAtAsync<GetAudiences>(new { entity.Id }, Response);
    }
}
