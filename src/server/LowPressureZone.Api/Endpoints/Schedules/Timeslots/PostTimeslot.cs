using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Entities;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PostTimeslot(
    DataContext dataContext,
    PerformerRules performerRules,
    ScheduleRules scheduleRules,
    AzuraCastClient client,
    UserManager<AppUser> userManager)
    : EndpointWithMapper<TimeslotRequest, TimeslotMapper>
{
    public override void Configure()
    {
        Post("/schedules/{scheduleId}/timeslots");
        AllowFormData();
        Description(builder => builder.Produces(201));
    }

    public override async Task HandleAsync(TimeslotRequest request, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var schedule = await dataContext.Schedules
                                        .Include(schedule => schedule.Timeslots)
                                        .Include(schedule => schedule.Community)
                                        .ThenInclude(community =>
                                                         community.Relationships.Where(relationship =>
                                                                                           relationship.UserId ==
                                                                                           User.GetIdOrDefault()))
                                        .Where(schedule => schedule.Id == scheduleId)
                                        .FirstAsync(ct);

        var performer = await dataContext.Performers.FirstAsync(p => p.Id == request.PerformerId, ct);
        var user = await userManager.GetUserAsync(User);
        if (user?.StreamerId is null
            || !scheduleRules.IsAddingTimeslotsAuthorized(schedule)
            || !performerRules.IsTimeslotLinkAuthorized(performer))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var timeslot = Map.ToEntity(request);
        dataContext.Timeslots.Add(timeslot);

        if (timeslot.Type == PerformanceTypes.Prerecorded && request.File is not null)
        {
            var createPrerecordedItemResult =
                await client.CreatePrerecordedItemAsync(user.StreamerId.Value, timeslot, request.File);
            if (!createPrerecordedItemResult.IsSuccess)
                ThrowError(createPrerecordedItemResult.Error.ReasonPhrase ??
                           "Error creating items in azuracast for timeslot");
        }

        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetScheduleById>(new
        {
            id = scheduleId
        }, Response, cancellation: ct);
    }
}
