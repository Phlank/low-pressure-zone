using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users;

public class PostUser : Endpoint<UserRequest, EmptyResponse>
{
    public required IdentityContext IdentityContext { get; set; }
    public required UserManager<IdentityUser> UserManager { get; set; }

    public override void Configure()
    {
        Post("/users/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var emailInUse = await IdentityContext.Users.AnyAsync(u => u.NormalizedEmail == req.Email.ToUpper());
        if (emailInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Email), "Email in use"));
        }
        var user = new IdentityUser
        {
            UserName = Guid.NewGuid().ToString(),
            Email = req.Email,
            NormalizedEmail = req.Email.ToUpper(),
            EmailConfirmed = false,
        };
        await UserManager.CreateAsync(user);
        await UserManager.AddPasswordAsync(user, Guid.NewGuid().ToString());
    }
}
