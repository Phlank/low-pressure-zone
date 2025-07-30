using System.Security.Claims;

namespace LowPressureZone.Api.Extensions;

public static class HttpContextAccessorExtensions
{
    public static ClaimsPrincipal? GetAuthenticatedUserOrDefault(this IHttpContextAccessor accessor)
    {
        if (!accessor.HasAuthenticatedUser()) return null;
        return accessor.HttpContext!.User;
    }

    private static bool HasAuthenticatedUser(this IHttpContextAccessor accessor) =>
        accessor.HttpContext?.User.Identity is { IsAuthenticated: true };

    public static T Resolve<T>(this IHttpContextAccessor accessor)
    {
        if (accessor.HttpContext == null) throw new ArgumentException($"{nameof(accessor)}.HttpContext is null");
        return (T?)accessor.HttpContext?.RequestServices.GetService(typeof(T)) ??
               throw new InvalidOperationException("Unable to resolve service");
    }

    public static Guid GetGuidRouteParameterOrDefault(this IHttpContextAccessor accessor, string paramName)
    {
        var param = accessor.HttpContext?.Request.RouteValues.GetValueOrDefault(paramName, null);
        if (param == null) return Guid.Empty;
        if (Guid.TryParse(param.ToString(), out var result)) return result;
        return Guid.Empty;
    }
}
