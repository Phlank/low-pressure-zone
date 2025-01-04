using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.User;

public sealed class CreateUser : Endpoint<CreateUserRequest>
{
    public override void Configure()
    {
        Post("/user");
        AllowAnonymous();
    }

    public override Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
