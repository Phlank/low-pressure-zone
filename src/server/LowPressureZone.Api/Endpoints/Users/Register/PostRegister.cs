using FastEndpoints;
using FluentEmail.Core;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Utilities;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Register;

public class PostRegister : Endpoint<RegisterRequest>
{
    public required UserManager<AppUser> UserManager { get; set; }
    public required SignInManager<AppUser> SignInManager { get; set; }
    public required IdentityContext IdentityContext { get; set; }
    private DateTime _requestTime = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/register");
        Throttle(5, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        RegistrationContext? context = null;
        try
        {
            context = RegistrationContext.Decode(req.Context);
        }
        catch (Exception)
        {
            await this.SendDelayedForbiddenAsync(_requestTime, ct);
            return;
        }

        var user = await UserManager.FindByEmailAsync(context.Email);
        if (user == null)
        {
            await this.SendDelayedForbiddenAsync(_requestTime, ct);
            return;
        }

        var invitation = await IdentityContext.Invitations.FirstOrDefaultAsync(i => i.UserId == user.Id);
        if (invitation == null)
        {
            await this.SendDelayedForbiddenAsync(_requestTime, ct);
            return;
        }
        
        var isUsernameInUse = await IdentityContext.Users.AnyAsync(u => u.NormalizedUserName == req.Username.ToUpper());
        if (isUsernameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Username), Errors.Unique));
        }

        var isValidToken = await UserManager.VerifyUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite, context.Token);
        if (!isValidToken)
        {
            await TaskUtilities.DelaySensitiveResponse(_requestTime);
            ThrowError(new ValidationFailure(null, Errors.ExpiredToken));
            return;
        }

        var setUsernameResult = await UserManager.SetUserNameAsync(user, req.Username);
        setUsernameResult.Errors.Select(e => e.Description).ForEach((message) =>
        {
            AddError(new ValidationFailure(nameof(req.Username), message));
        });
        ThrowIfAnyErrors();
        
        var setPasswordResult = await UserManager.AddPasswordAsync(user, req.Password);
        setPasswordResult.Errors.Select(e => e.Description).ForEach((message) =>
        {
            AddError(new ValidationFailure(nameof(req.Password), message));
        });
        ThrowIfAnyErrors();

        user.EmailConfirmed = true;
        await UserManager.UpdateAsync(user);

        invitation.IsRegistered = true;
        invitation.RegistrationDate = DateTime.UtcNow;
        await IdentityContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}
