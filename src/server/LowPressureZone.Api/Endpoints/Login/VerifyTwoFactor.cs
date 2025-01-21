using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Login;

public class VerifyTwoFactor : Endpoint<VerifyTwoFactorRequest, VerifyTwoFactorResponse>
{
    public override void Configure()
    {
        Post("/login/verifytwofactor");
    }
}
