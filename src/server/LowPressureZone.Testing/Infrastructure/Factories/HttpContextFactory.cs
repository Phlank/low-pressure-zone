using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FakeItEasy;
using LowPressureZone.Testing.Infrastructure.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LowPressureZone.Testing.Infrastructure.Factories;

public static class HttpContextFactory
{
    public static (HttpContext, IHttpContextAccessor) Create(
        ClaimsPrincipal? user = null,
        string? path = null,
        string? method = null,
        IServiceCollection? services = null)
    {
        var context = new DefaultHttpContext();
        var accessor = A.Fake<IHttpContextAccessor>();
        A.CallTo(() => accessor.HttpContext).Returns(context);

        if (user is not null)
            context.User = user;

        if (path is not null)
            context.Request.Path = path;

        if (method is not null)
            context.Request.Method = method;

        if (services is not null)
            context.RequestServices = services.BuildServiceProvider();
        else
            context.RequestServices = new ServiceCollection().BuildServiceProvider();

        return (context, accessor);
    }

    public static (HttpContext, IHttpContextAccessor) Create<T>(
        T requestBody,
        ClaimsPrincipal? user = null,
        string? path = null,
        string? method = null)
    {
        var (context, accessor) = Create(user, path, method);
        
        context.Request.ContentType = "application/json";
        var serialized = JsonSerializer.Serialize(requestBody);
        var byteArray = Encoding.UTF8.GetBytes(serialized);
        context.Request.Body = new MemoryStream(byteArray);
        context.Request.Headers.ContentEncoding = "utf-8";

        return (context, accessor);
    }
}