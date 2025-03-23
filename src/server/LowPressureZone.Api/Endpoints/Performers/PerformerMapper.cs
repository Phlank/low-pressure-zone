using System.Security.Claims;
using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerMapper(IHttpContextAccessor contextAccessor, PerformerRules rules) : IRequestMapper, IResponseMapper
{
    private ClaimsPrincipal? User => contextAccessor.GetAuthenticatedUserOrDefault();

    public Performer ToEntity(PerformerRequest request)
    {
        User.ShouldNotBeNull();

        return new Performer
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Url = request.Url?.Trim(),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            LinkedUserIds = [User.GetIdOrDefault()]
        };
    }

    public async Task UpdateEntityAsync(PerformerRequest request, Performer performer, CancellationToken ct = default)
    {
        var dataContext = contextAccessor.Resolve<DataContext>();
        performer.Name = request.Name;
        performer.Url = request.Url;
        if (!dataContext.ChangeTracker.HasChanges()) return;
        performer.LastModifiedDate = DateTime.UtcNow;
        await dataContext.SaveChangesAsync(ct);
    }

    public PerformerResponse FromEntity(Performer performer)
        => new PerformerResponse
        {
            Id = performer.Id,
            Name = performer.Name,
            Url = performer.Url,
            IsDeletable = rules.IsDeleteAuthorized(performer) && !performer.IsDeleted,
            IsEditable = rules.IsEditAuthorized(performer),
            IsLinkableToTimeslot = rules.IsTimeslotLinkAuthorized(performer)
        };
}
