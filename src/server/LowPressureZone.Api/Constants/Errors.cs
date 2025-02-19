namespace LowPressureZone.Api.Constants;

// These get mapped onto field labels, so they should remain as short as possible while being descriptive enough to solve the issues
public static class Errors
{
    public const string Required = "Required";
    public const string Unique = "Already in use";
    public const string InvalidUrl = "Invalid URL";
    public const string InvalidEmail = "Invalid email";

    public const string EmailAlreadyInvited = "Already invited";
}
