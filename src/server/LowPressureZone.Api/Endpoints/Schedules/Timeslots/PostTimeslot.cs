using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Core;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Migrations;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PostTimeslot(DataContext dataContext, FormFileSaver fileSaver, PerformerRules performerRules, ScheduleRules scheduleRules)
    : EndpointWithMapper<TimeslotRequest, TimeslotMapper>
{
    public override void Configure()
    {
        AllowFormData();
        AllowFileUploads();
        Post("/schedules/{scheduleId}/timeslots");
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
        
        if (!scheduleRules.IsAddingTimeslotsAuthorized(schedule)
            || !performerRules.IsTimeslotLinkAuthorized(performer))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        if (request is { PerformanceType: PerformanceTypes.Prerecorded, File: not null })
        {
            var saveFileResult = await fileSaver.SaveFormFileAsync(request.File, ct);
        }

        var timeslot = Map.ToEntity(request);
        dataContext.Timeslots.Add(timeslot);
        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await SendCreatedAtAsync<GetScheduleById>(new
        {
            id = scheduleId
        }, Response, cancellation: ct);
    }
}