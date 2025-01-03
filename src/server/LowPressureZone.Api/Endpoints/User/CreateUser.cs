using FastEndpoints;
using static LowPressureZone.Api.Endpoints.User.CreateUser;

namespace LowPressureZone.Api.Endpoints.User;

public partial class CreateUser : Endpoint<Request>
{
    public override void Configure()
    {
        Post("/user");
        AllowAnonymous();
    }

    public override Task HandleAsync(Request req, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
