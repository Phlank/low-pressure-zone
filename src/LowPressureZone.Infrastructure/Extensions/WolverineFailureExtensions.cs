using LowPressureZone.Infrastructure.Emails;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.ErrorHandling;

namespace LowPressureZone.Infrastructure.Extensions;

public static class WolverineFailureExtensions
{
    public static IAdditionalActions SendAdminEmail(this IFailureActions actions) =>
        actions.CustomAction(async (runtime,
                                    lifecycle,
                                    ex) =>
                             {
                                 var email = runtime.Services.GetRequiredService<IEmailService>();
                                 await email.SendAdminException(ex,
                                                                "Message Failure",
                                                                $"Message type: {lifecycle.Envelope?.MessageType}");
                             },
                             "Email administrator");
}