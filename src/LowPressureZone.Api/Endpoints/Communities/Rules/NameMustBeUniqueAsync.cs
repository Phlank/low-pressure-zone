using FastEndpoints;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Domain.CommunityAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Rules;

[RegisterService<NameMustBeUniqueRule>(LifeTime.Scoped)]
public class NameMustBeUniqueRule(DataContext dataContext) : IAsyncRule<Community>
{
    public async Task<bool> IsBrokenAsync(Community entity, CancellationToken ct = default) =>
        await dataContext.Communities.AnyAsync(c => c.Name == entity.Name && c.Id != entity.Id, ct);

    public RuleError Error => new("Already in use", nameof(Community.Name));
}