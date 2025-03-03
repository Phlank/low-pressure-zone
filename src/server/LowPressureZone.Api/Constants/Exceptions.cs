namespace LowPressureZone.Api.Constants;

public static class Exceptions
{
    public static readonly Exception NoAuthorizedUserForToEntityMap = new InvalidOperationException("Cannot map request to domain entity without authorized user");
}
