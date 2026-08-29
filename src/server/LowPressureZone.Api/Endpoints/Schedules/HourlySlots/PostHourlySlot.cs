using System.Diagnostics;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Domain.ScheduleAggregate;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class PostHourlySlot(
    DataContext dataContext,
    ScheduleRules scheduleRules,
    PerformerRules performerRules,
    HourlySlotRules hourlyRules,
    HourlySlotPrerecordedMixHandler mixHandler) : Endpoint<HourlySlotRequest>
{
    public override void Configure()
    {
        Post("/schedules/{id}/hourly-slots");
        AllowFormData();
        AllowFileUploads();
        Description(b => b.Produces(201));
        Roles(RoleNames.AllRoles);
    }

    public override async Task HandleAsync(HourlySlotRequest request, CancellationToken ct)
    {
        var id = Route<Guid>("id");

        var schedule = await dataContext.NewSchedules
                                        .Where(schedule => schedule.Id == id)
                                        .FirstOrDefaultAsync(ct);

        if (schedule is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var performer = await dataContext.Performers
                                         .Where(performer => performer.Id == request.PerformerId)
                                         .FirstOrDefaultAsync(ct);
        if (performer is null || !performerRules.IsHourlySlotLinkAuthorized(performer) ||
            !scheduleRules.IsAddingHourlySlotsAllowed(schedule))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var prerecordedMixResult = request.File is not null
                                       ? await mixHandler.AddNewMixFile(this, schedule, request, ct)
                                       : null;
        if (prerecordedMixResult is not null)
            this.ThrowIfDomainError(prerecordedMixResult);

        var slotResult = HourlySlot.Create(schedule.Id,
                                           request.PerformerId,
                                           request.StartsAt,
                                           request.Duration,
                                           request.Subtitle,
                                           prerecordedMixResult?.Value.UploadedFileName,
                                           prerecordedMixResult?.Value.AzuraCastMediaId);
        this.ThrowIfDomainError(slotResult);

        var addResult = schedule.AddHourlySlot(slotResult.Value);
        this.ThrowIfDomainError(addResult);

        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetHourlySlotById>(new { slotResult.Value.Id }, cancellation: ct);
    }
}