using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core.Domain;
using LowPressureZone.Scheduling.Domain.PerformerAggregate;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Performers;

[RegisterService<PerformerMapper>(LifeTime.Singleton)]
public sealed class PerformerMapper(IHttpContextAccessor contextAccessor, PerformerPermissions permissions)
    : IRequestMapper, IResponseMapper
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public DomainResult<Performer> ToEntity(PerformerRequest request)
    {
        User.ShouldNotBeNull();
        return Performer.Create(User.GetIdOrDefault(), request.Name, request.SocialUrl);
    }

    public PerformerResponse FromEntity(Performer performer)
        => new()
        {
            Id = performer.Id,
            Name = performer.Name,
            SocialUrl = performer.SocialUrl,
            IsDeletable = permissions.IsDeleteAuthorized(performer) && !performer.IsDeleted,
            IsEditable = permissions.IsEditAuthorized(performer),
            IsLinkableToTimeslot = permissions.IsHourlySlotLinkAuthorized(performer)
        };
}