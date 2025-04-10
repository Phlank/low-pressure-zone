namespace LowPressureZone.Api.Extensions;

public static class HttpContextExtensions
{
    public static void ExposeLocation(this HttpContext context) => context.Response.Headers.Append("Access-Control-Expose-Headers", "location");
}
