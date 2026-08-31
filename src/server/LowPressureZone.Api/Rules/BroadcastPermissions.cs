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

    public bool IsDownloadable(StationStreamerBroadcast broadcast)
        => broadcast.Recording is not null;

    public bool IsDeletable(StationStreamerBroadcast broadcast)
        => User is not null && User.IsInRole(RoleNames.Admin);

    public bool IsDisconnectable(StationStreamerBroadcast broadcast) =>
        User is not null 
        && (User.IsInRole(RoleNames.Admin) 
            || User.IsInRole(RoleNames.Organizer)) 
        && broadcast.TimestampEnd is null;

    public bool IsArchivable(
        StationStreamerBroadcast externalBroadcast,
        Broadcast? broadcast) =>
        User is not null &&
        (User.IsInRole(RoleNames.Admin)
            || User.IsInRole(RoleNames.Organizer))
        && !string.IsNullOrEmpty(externalBroadcast.Recording?.DownloadUrl)
        && broadcast is not { IsArchived: true };
}