using FastEndpoints;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users;

public class GetUsers : EndpointWithoutRequest<IEnumerable<GetUserResponse>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IdentityContext _identityContext;

    public GetUsers(UserManager<IdentityUser> userManager, IdentityContext identityContext)
    {
        _userManager = userManager;
        _identityContext = identityContext;
    }

    public override void Configure()
    {
        Get("/users");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = _userManager.Users.Select(user => new GetUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            Username = user.UserName
        });
        await SendOkAsync(response);
    }
}
