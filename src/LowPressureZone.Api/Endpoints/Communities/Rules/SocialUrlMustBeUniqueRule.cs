using FastEndpoints;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Domain.CommunityAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Rules;

[RegisterService<SocialUrlMustBeUniqueRule>(LifeTime.Scoped)]
public class SocialUrlMustBeUniqueRule(DataContext dataContext) : IAsyncRule<Community>
{
    public async Task<bool> IsBrokenAsync(Community entity, CancellationToken ct = default) =>
        await dataContext.Communities.AnyAsync(c => c.SocialUrl == entity.SocialUrl && c.Id != entity.Id && !c.IsDeleted, ct);

    public RuleError Error => new("Already in use", nameof(Community.SocialUrl));
}