using System.Security.Claims;
using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Rules;

public class AudienceRules
{
    private readonly IHttpContextAccessor _contextAccessor;
    private ClaimsPrincipal? User => _contextAccessor.GetAuthenticatedUserOrDefault();
    private DataContext DataContext => _contextAccessor.Resolve<DataContext>();

    public AudienceRules(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public bool IsScheduleLinkAuthorized(Audience audience)
    {
        if (User == null) return false;
        if (User.IsInRole(RoleNames.Admin)) return true;
        return User.IsInRole(RoleNames.Organizer) && audience.LinkedUserIds.Contains(User.GetIdOrDefault());
    }

    public bool IsEditAuthorized(Audience audience)
    {
        if (User == null) return false;
        return User.IsInRole(RoleNames.Admin);
    }

    public bool IsDeleteAuthorized(Audience audience)
    {
        if (User == null) return false;
        return User.IsInRole(RoleNames.Admin);
    }

    public async Task<bool> IsDeleteValidAsync(Audience audience, CancellationToken ct) => !(await GetDeleteValidationErrorsAsync(audience, ct)).Any();

    public async Task ValidateDeleteAsync(Audience audience, CancellationToken ct) 
        => ValidationContext<AudienceRequest>.Instance.ValidationFailures.AddRange(await GetDeleteValidationErrorsAsync(audience, ct));

    private async Task<IEnumerable<ValidationFailure>> GetDeleteValidationErrorsAsync(Audience audience, CancellationToken ct)
    {
        var result = new List<ValidationFailure>();
        if (!await IsNotLinkedToSchedulesAsync(audience, ct)) result.Add(AudienceLinkedToScheduleFailure);
        return result;
    }

    private static readonly ValidationFailure AudienceLinkedToScheduleFailure = new ValidationFailure(null, "Cannot delete an audience when it is linked to schedules");
    private async Task<bool> IsNotLinkedToSchedulesAsync(Audience audience, CancellationToken ct)
        => !await DataContext.Schedules.AnyAsync(s => s.AudienceId == audience.Id, ct);
}
