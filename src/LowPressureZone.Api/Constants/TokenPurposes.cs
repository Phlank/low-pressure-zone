using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Constants;

internal static class TokenPurposes
{
    public const string Invite = "Invite";
    public const string ResetPassword = UserManager<AppUser>.ResetPasswordTokenPurpose;
}