using System.Globalization;
using System.Text;
using FastEndpoints;
using LowPressureZone.Api.Utilities;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using Wolverine;
using IEvent = LowPressureZone.Core.Domain.IEvent;

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
        Result<T, string> result,
        CompositeFormat format) where TRequest : notnull
    {
        if (result.IsError)
            endpoint.ThrowError(string.Format(CultureInfo.InvariantCulture, format, result.Error));
    }

    public static async Task PublishOrThrowAsync<T, TRequest, TResponse>(
        this Endpoint<TRequest, TResponse> endpoint,
        DomainResult<T> result) where TRequest : notnull
    {
        endpoint.ThrowIfDomainErrors(result);
        var messageBus = endpoint.Resolve<IMessageBus>();
        foreach (var @event in result.Events)
        {
            await messageBus.PublishAsync(@event);
        }
    }
    
    public static async Task PublishEventsAsync<TRequest, TResponse>(
        this Endpoint<TRequest, TResponse> endpoint,
        List<IEvent> events) where TRequest : notnull
    {
        var messageBus = endpoint.Resolve<IMessageBus>();
        foreach (var @event in events)
        {
            await messageBus.PublishAsync(@event);
        }
    }

    public static void ThrowIfDomainErrors<T, TRequest, TResponse>(
        this Endpoint<TRequest, TResponse> endpoint,
        DomainResult<T> result) where TRequest : notnull
    {
        foreach (var failure in result.Failures)
            endpoint.AddError(failure);

        endpoint.ThrowIfAnyErrors();
    }

    public static void ThrowIfRuleErrors<TRequest, TResponse>(this Endpoint<TRequest, TResponse> endpoint,
                                                              List<RuleError> errors) where TRequest : notnull
    {
        foreach (var error in errors)
            endpoint.AddError(error.ToValidationFailure());

        endpoint.ThrowIfAnyErrors();
    }
}