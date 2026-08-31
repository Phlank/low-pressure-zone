using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain.BroadcastAggregate;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Rules;

[RegisterService<BroadcastPermissions>(LifeTime.Singleton)]
public sealed class BroadcastPermissions(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsDownloadable(Broadcast broadcast)
        => broadcast.HasFile;

    public bool IsDeletable(Broadcast broadcast)
        => User is not null && User.IsInRole(RoleNames.Admin);

    public bool IsDisconnectable(Broadcast broadcast) =>
        User is not null
        && (User.IsInRole(RoleNames.Admin)
            || User.IsInRole(RoleNames.Organizer))
        && broadcast.Time.EndsAt is null;

    public bool IsArchivable(
        Broadcast broadcast) =>
        User is not null
        && (User.IsInRole(RoleNames.Admin)
            || User.IsInRole(RoleNames.Organizer))
        && broadcast is { IsArchived: false, HasFile: true };
}