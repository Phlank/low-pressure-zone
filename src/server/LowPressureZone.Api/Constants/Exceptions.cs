namespace LowPressureZone.Api.Constants;

public static class Exceptions
{
    public static readonly Exception NoAuthorizedUserInToEntityMap = new InvalidOperationException("Cannot map request to domain entity without authorized user");
}
