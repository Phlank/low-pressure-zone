using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Extensions;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users;

public sealed class CreateUser : Endpoint<CreateUserRequest>
{
    private readonly UserManager<IdentityUser> _userManager;
    public CreateUser(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
        Description(builder => builder.Produces(201)
                                      .ProducesProblemDetails(400, "application/json+problem"));
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var existingUser = await _userManager.FindByNameAsync(req.Username);
        if (existingUser is not null)
        {
            ThrowError(new ValidationFailure(nameof(req.Username), "Username in use. Choose a different username."));
        }
        existingUser = await _userManager.FindByEmailAsync(req.Email);
        if (existingUser is not null)
        {
            ThrowError(new ValidationFailure(nameof(req.Email), "A user is already associated with this email."));
        }

        IdentityUser newUser = new()
        {
            UserName = req.Username,
            Email = req.Email,
            TwoFactorEnabled = false,
        };
        var result = await _userManager.CreateAsync(newUser);
        this.ThrowIfIdentityErrors(result);

        var user = await _userManager.FindByNameAsync(req.Username);
        result = await _userManager.AddPasswordAsync(user!, req.Password);
        if (!result.Succeeded)
        {
            await _userManager.DeleteAsync(user!);
            this.ThrowIfIdentityErrors(result, nameof(req.Password));
            return;
        }

        await SendCreatedAtAsync<GetUser>(new { user!.Id }, null, cancellation: ct);
    }
}
