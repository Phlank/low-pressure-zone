using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public static class SoundclashQueries
{
    public static IQueryable<Soundclash> GetSoundclashesForResponse(this IQueryable<Soundclash> query) =>
        query.Include(sc => sc.PerformerOne)
             .Include(sc => sc.PerformerTwo)
             .AsNoTracking();

    public static IQueryable<Soundclash> GetSoundclashForUpdate(
        this IQueryable<Soundclash> query,
        Guid soundclashId,
        Guid userId) =>
        query.Include(soundclash => soundclash.Schedule)
             .ThenInclude(schedule => schedule.Community)
             .ThenInclude(community => community.Relationships.Where(relationship => relationship.IsOrganizer &&
                                                                                     relationship.UserId == userId))
             .Where(soundclash => soundclash.Id == soundclashId);
}