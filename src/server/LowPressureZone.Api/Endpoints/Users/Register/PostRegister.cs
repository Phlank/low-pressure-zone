using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Register;

public class PostRegister : Endpoint<RegisterRequest>
{
    public required UserManager<IdentityUser> UserManager { get; set; }
    public required SignInManager<IdentityUser> SignInManager { get; set; }
    public required IdentityContext IdentityContext { get; set; }

    public override void Configure()
    {
        Post("/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Context))
        {
            Logger.LogError("{ClassName}: Registration attempted with empty or null context.", nameof(PostRegister));
            await SendForbiddenAsync(ct);
            return;
        }
        RegistrationContext? context = null;
        try
        {
            context = RegistrationContext.Decode(req.Context);
            if (context == null)
            {
                await SendForbiddenAsync(ct);
                return;
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "{ClassName}: Registration attempted with invalid context.", nameof(PostRegister));
            await SendForbiddenAsync(ct);
            return;
        }

        var user = await UserManager.FindByEmailAsync(context.Email);
        if (user == null)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        var isValidToken = await UserManager.VerifyUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite, context.Token);
        if (!isValidToken)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        var isUsernameInUse = await IdentityContext.Users.AnyAsync(u => u.NormalizedUserName == req.Username.ToUpper());
        if (isUsernameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Username), Errors.Unique));
        }
        await UserManager.SetUserNameAsync(user, req.Username);
        await UserManager.AddPasswordAsync(user, req.Password);
        await SignInManager.SignInAsync(user, false);
        await SendOkAsync(ct);
    }
}
