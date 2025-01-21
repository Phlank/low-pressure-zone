using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Token;

public class Login : Endpoint<LoginRequest, LoginResponse>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly string _signingKey;
    public Login(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
        _signingKey = Config.GetValue<string>("JwtSigningKey") ?? throw new NullReferenceException();
    }

    public override void Configure()
    {
        Get("/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user is null)
        {
            await SendUnauthorizedAsync();
            return;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!isPasswordValid)
        {
            await SendUnauthorizedAsync();
            return;
        }

        if (user.TwoFactorEnabled)
        {
            // TODO: Implement 2FA
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = _signingKey;
            options.ExpireAt = DateTime.UtcNow.AddDays(1);
            options.User.Roles.AddRange(roles);
            options.User["Id"] = user.Id;
        });
        await SendOkAsync(new LoginResponse
        {
            Token = token,
            Username = req.Username
        }, ct);
    }
}
