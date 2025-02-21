using FastEndpoints;
using LowPressureZone.Api.Utilities;

namespace LowPressureZone.Api.Extensions;

public static class EndpointExtensions
{
    public static async Task SendDelayedUnauthorizedAsync(this BaseEndpoint endpoint, DateTime requestTime, CancellationToken ct = default)
    {
        await TaskUtilities.DelaySensitiveResponse(requestTime);
        await endpoint.HttpContext.Response.SendUnauthorizedAsync(ct);
    }

    public static async Task SendDelayedForbiddenAsync(this BaseEndpoint endpoint, DateTime requestTime, CancellationToken ct = default)
    {
        await TaskUtilities.DelaySensitiveResponse(requestTime);
        await endpoint.HttpContext.Response.SendForbiddenAsync(ct);
    }
}
