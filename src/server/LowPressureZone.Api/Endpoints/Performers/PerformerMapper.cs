using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerMapper(IHttpContextAccessor contextAccessor, PerformerRules rules)
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
            IsDeletable = rules.IsDeleteAuthorized(performer) && !performer.IsDeleted,
            IsEditable = rules.IsEditAuthorized(performer),
            IsLinkableToTimeslot = rules.IsTimeslotLinkAuthorized(performer)
        };
}