using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LowPressureZone.Domain.Extensions;

public static class HttpContextAccessorExtensions
{
    public static bool HasAuthenticatedUser(this IHttpContextAccessor accessor) => accessor.HttpContext != null
                                                                                   && accessor.HttpContext.User.Identity != null
                                                                                   && accessor.HttpContext.User.Identity.IsAuthenticated;

    public static ClaimsPrincipal? GetAuthenticatedUserOrDefault(this IHttpContextAccessor accessor)
    {
        if (!accessor.HasAuthenticatedUser()) return null;
        return accessor.HttpContext!.User;
    }

    public static DataContext ResolveDataContext(this IHttpContextAccessor accessor)
    {
        return (DataContext?)accessor.HttpContext?.RequestServices.GetService(typeof(DataContext)) ?? throw new ArgumentException(nameof(accessor), "HttpContext is null");
    }
}
