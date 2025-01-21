using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Extensions;

public static class EndpointExtensions
{
    public static void ThrowIfIdentityErrors<TRequest, TResponse>(this Endpoint<TRequest, TResponse> endpoint, IdentityResult identityResult, string? propertyName = null) where TRequest : notnull
    {
        if (identityResult.Succeeded) return;
        var failures = identityResult.ToValidationFailures(propertyName);
        endpoint.ValidationFailures.AddRange(failures);
        endpoint.ThrowIfAnyErrors();
    }

    private static IEnumerable<ValidationFailure> ToValidationFailures(this IdentityResult identityResult, string? propertyName = null)
    {
        return identityResult.Errors.Select((identityResult) => identityResult.ToValidationFailure(propertyName));
    }

    private static ValidationFailure ToValidationFailure(this IdentityError error, string? propertyName)
    {
        return new ValidationFailure
        {
            PropertyName = propertyName,
            ErrorCode = error.Code,
            ErrorMessage = error.Description
        };
    }
}