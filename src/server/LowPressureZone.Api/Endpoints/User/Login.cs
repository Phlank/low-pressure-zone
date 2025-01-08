using FastEndpoints;
using FastEndpoints.Security;
using LowPressureZone.Api.StaticUtils;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.User;

public sealed class Login : Endpoint<LoginRequest>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    private const int DELAY_MILLISECONDS = 3000;

    public Login(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public override void Configure()
    {
        Post("/user/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var startTime = DateTime.UtcNow;
        var user = await _userManager.FindByNameAsync(req.UserName);
        if (user == null)
        {
            await TaskUtils.DelayFromTime(startTime, DELAY_MILLISECONDS);
            await SendUnauthorizedAsync();
            return;
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, req.Password, true, false);
        if (!signInResult.Succeeded)
        {
            await TaskUtils.DelayFromTime(startTime, DELAY_MILLISECONDS);
            await SendUnauthorizedAsync();
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);
        await SendAsync(JwtBearer.CreateToken(options =>
        {
            options.ExpireAt = DateTime.UtcNow.AddDays(1);
            options.User.Roles.AddRange(roles);
            options.User.Claims.Add(("name", req.UserName));
        }));
    }
}
