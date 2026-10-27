using LowPressureZone.Api.Services;

namespace LowPressureZone.Api.Filters;

public class AdminEmailExceptionFilter(EmailService emailService, ILogger<AdminEmailExceptionFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (Exception ex)
        {
            try
            {
                await emailService.SendAdminExceptionAsync(ex, context.HttpContext.Request.Path);
            }
            catch (Exception ex2)
            {
                logger.LogError(ex2, "Exception thrown during execution of the admin exception email sending.");
                logger.LogError(ex, "Original exception handled in filter.");
            }
        }

        return ValueTask.CompletedTask;
    }
}