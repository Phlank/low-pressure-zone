using FastEndpoints;
using static LowPressureZone.Api.Endpoints.User.CreateUser;

namespace LowPressureZone.Api.Endpoints.User;

public partial class CreateUser : Endpoint<Request>
{
    public override Task HandleAsync(Request req, CancellationToken ct)
    {

    }
}
