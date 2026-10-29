using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LowPressureZone.Infrastructure.Events;

public class EventMiddleware(IEventQueue queue) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        // Flush events on successful responses
        if (context.Response.StatusCode is < 200 or >= 300)
            return;
        
        var eventQueue = context.RequestServices.GetRequiredService<IEventQueue>();
        await eventQueue.FlushAsync();
    }
}