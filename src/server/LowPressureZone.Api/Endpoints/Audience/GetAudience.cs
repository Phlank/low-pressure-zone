using FastEndpoints;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Audience;

public class GetAudience : Endpoint<EmptyRequest, IEnumerable<GetAudienceResponse>, AudienceDtoMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/audience");
        Description(b => b.Produces<IEnumerable<GetAudienceResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var audiences = DataContext.Audiences.AsEnumerable();
        await SendOkAsync(audiences.Select(aud => Map.FromEntity(aud)));
    }
}
