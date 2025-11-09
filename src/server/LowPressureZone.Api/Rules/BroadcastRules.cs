using System.Security.Claims;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Rules;

public class BroadcastRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsDownloadable(StationStreamerBroadcast broadcast)
        => broadcast.Recording is not null;

    public bool IsDeletable(StationStreamerBroadcast broadcast)
        => User is not null && User.IsInRole(RoleNames.Admin);
}