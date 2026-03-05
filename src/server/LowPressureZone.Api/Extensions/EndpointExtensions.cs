using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using FastEndpoints;
using LowPressureZone.Api.Utilities;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Extensions;

public static class EndpointExtensions
{
    public static async Task SendDelayedUnauthorizedAsync(
        this BaseEndpoint endpoint,
        DateTime requestTime,
        CancellationToken ct = default)
    {
        await TaskUtilities.DelaySensitiveResponse(requestTime);
        await endpoint.HttpContext.Response.SendUnauthorizedAsync(ct);
    }

    public static async Task SendDelayedForbiddenAsync(
        this BaseEndpoint endpoint,
        DateTime requestTime,
        CancellationToken ct = default)
    {
        await TaskUtilities.DelaySensitiveResponse(requestTime);
        await endpoint.HttpContext.Response.SendForbiddenAsync(ct);
    }

    public static async Task SendDelayedNoContentAsync(
        this BaseEndpoint endpoint,
        DateTime requestTime,
        CancellationToken ct = default)
    {
        await TaskUtilities.DelaySensitiveResponse(requestTime);
        await endpoint.HttpContext.Response.SendNoContentAsync(ct);
    }

    public static void ThrowIfError<T, TRequest, TResponse>(
        this Endpoint<TRequest, TResponse> endpoint,
        Result<T, string> result) where TRequest : notnull
    {
        if (result.IsError)
            endpoint.ThrowError(result.Error);
    }

    public static void ThrowIfError<T, TRequest, TResponse>(
        this Endpoint<TRequest, TResponse> endpoint,
        Result<T, string> result,
        CompositeFormat format) where TRequest : notnull
    {
        if (result.IsError)
            endpoint.ThrowError(string.Format(CultureInfo.InvariantCulture, format, result.Error));
    }

    public static void ThrowIfError<T, TRequest, TResponse>(
        this Endpoint<TRequest, TResponse> endpoint,
        Result<T, HttpResponseMessage> result,
        CompositeFormat format) where TRequest : notnull
    {
        if (result.IsError)
            endpoint.ThrowError(string.Format(CultureInfo.InvariantCulture, format, result.Error.ReasonPhrase));
    }
}