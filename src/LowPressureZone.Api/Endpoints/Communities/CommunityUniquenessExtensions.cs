using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Data;
using LowPressureZone.Domain.CommunityAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

internal static class CommunityUniquenessExtensions
{
    /// <summary>
    ///     Throws a validation error when another community, including a deleted one, already uses the name or social URL
    ///     of <paramref name="community" />. Both are covered by unique database indexes.
    /// </summary>
    public static async Task ThrowIfNotUniqueAsync(
        this Endpoint<CommunityRequest> endpoint,
        DataContext dataContext,
        Community community,
        CancellationToken ct)
    {
        var conflicts = await dataContext.Communities
                                         .AsNoTracking()
                                         .Where(other => other.Id != community.Id
                                                         && (other.Name == community.Name
                                                             || other.SocialUrl == community.SocialUrl))
                                         .Select(other => new { other.Name, other.SocialUrl })
                                         .ToListAsync(ct);

        if (conflicts.Any(other => other.Name == community.Name))
            endpoint.AddError(new ValidationFailure(nameof(CommunityRequest.Name), "Name is already in use"));

        if (conflicts.Any(other => other.SocialUrl == community.SocialUrl))
            endpoint.AddError(new ValidationFailure(nameof(CommunityRequest.SocialUrl), "Social URL is already in use"));

        endpoint.ThrowIfAnyErrors();
    }
}
