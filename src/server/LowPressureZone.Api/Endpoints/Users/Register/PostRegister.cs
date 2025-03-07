using System.Text.Json;
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

public class PostRegister(UserManager<AppUser> userManager, IdentityContext identityContext) : Endpoint<RegisterRequest>
{
    private readonly DateTime _requestTime = DateTime.UtcNow;

    public override void Configure()
    {
        Post("/users/register");
        Throttle(5, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        TokenContext? context = null;
        try
        {
            context = TokenContext.Decode(req.Context);
            if (context == null)
            {
                await this.SendDelayedForbiddenAsync(_requestTime, ct);
                return;
            }
        }
        catch (JsonException)
        {
            await this.SendDelayedForbiddenAsync(_requestTime, ct);
            throw;
        }

        var user = await userManager.FindByEmailAsync(context.Email);
        if (user == null)
        {
            await this.SendDelayedForbiddenAsync(_requestTime, ct);
            return;
        }

        var invitation = await identityContext.Invitations.FirstOrDefaultAsync(i => i.UserId == user.Id, ct);
        if (invitation == null)
        {
            await this.SendDelayedForbiddenAsync(_requestTime, ct);
            return;
        }

        var normalizedRequestUsername = req.Username.ToUpperInvariant();
        var isUsernameInUse = await identityContext.Users.AnyAsync(u => u.NormalizedUserName == normalizedRequestUsername, ct);
        if (isUsernameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Username), Errors.Unique));
        }

        var isValidToken = await userManager.VerifyUserTokenAsync(user, TokenProviders.Default, TokenPurposes.Invite, context.Token);
        if (!isValidToken)
        {
            await TaskUtilities.DelaySensitiveResponse(_requestTime);
            ThrowError(new ValidationFailure(null, Errors.ExpiredToken));
            return;
        }

        var setUsernameResult = await userManager.SetUserNameAsync(user, req.Username);
        setUsernameResult.Errors.Select(e => e.Description).ForEach((message) =>
        {
            AddError(new ValidationFailure(nameof(req.Username), message));
        });
        ThrowIfAnyErrors();
        
        var setPasswordResult = await userManager.AddPasswordAsync(user, req.Password);
        setPasswordResult.Errors.Select(e => e.Description).ForEach((message) =>
        {
            AddError(new ValidationFailure(nameof(req.Password), message));
        });
        ThrowIfAnyErrors();

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);

        invitation.IsRegistered = true;
        invitation.RegistrationDate = DateTime.UtcNow;
        await identityContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}
