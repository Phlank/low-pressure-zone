using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerMapper(IHttpContextAccessor contextAccessor, PerformerRules rules) : Mapper<PerformerRequest, PerformerResponse, Performer>, IRequestMapper, IResponseMapper
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public override Performer ToEntity(PerformerRequest req)
    {
        User.ShouldNotBeNull();

        return new Performer
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Url = req.Url.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            LinkedUserIds = [User.GetIdOrDefault()]
        };
    }

    public override Task<Performer> ToEntityAsync(PerformerRequest req, CancellationToken ct = default)
        => Task.FromResult(ToEntity(req));

    public override async Task<Performer> UpdateEntityAsync(PerformerRequest req, Performer performer, CancellationToken ct = default)
    {
        var dataContext = Resolve<DataContext>();
        performer.Name = req.Name;
        performer.Url = req.Url;
        if (dataContext.ChangeTracker.HasChanges())
        {
            performer.LastModifiedDate = DateTime.UtcNow;
            await dataContext.SaveChangesAsync(ct);
        }
        return performer;
    }

    public override PerformerResponse FromEntity(Performer performer)
    {
        return new PerformerResponse
        {
            Id = performer.Id,
            Name = performer.Name,
            Url = performer.Url,
            IsDeletable = rules.IsDeleteAuthorized(performer) && !performer.IsDeleted,
            IsEditable = rules.IsEditAuthorized(performer),
            IsLinkableToTimeslot = rules.IsTimeslotLinkAuthorized(performer),
        };
    }

    public override Task<PerformerResponse> FromEntityAsync(Performer performer, CancellationToken ct = default)
        => Task.FromResult(FromEntity(performer));
}
