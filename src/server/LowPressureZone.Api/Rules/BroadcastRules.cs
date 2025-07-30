using System.Security.Claims;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models.Stream.AzuraCast.Schema;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Rules;

public class BroadcastRules(IHttpContextAccessor contextAccessor)
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public bool IsDownloadable(Broadcast broadcast)
        => broadcast.Recording is not null;

    public bool IsDeletable(Broadcast broadcast)
        => User is not null && User.IsInRole(RoleNames.Admin);
}