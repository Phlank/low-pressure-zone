using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class PutHourlySlot(DataContext dataContext, HourlySlotPrerecordedMixHandler mixHandler) : Endpoint<HourlySlotRequest>
{
    public override void Configure()
    {
        Put("/schedules/{scheduleId}/hourly-slots/{id}");
        AllowFormData();
        AllowFileUploads();
        Description(b => b.Produces(201));
        Roles(RoleNames.AllRoles);
    }

    public override async Task HandleAsync(HourlySlotRequest request, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var id = Route<Guid>("id");

        var schedule = await dataContext.NewSchedules
                                        .Where(schedule => schedule.Id == scheduleId)
                                        .FirstOrDefaultAsync(ct);
        
        var slot = schedule?.HourlySlots.FirstOrDefault(slot => slot.Id == id);

        if (schedule is null || slot is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var addMix = !slot.Prerecord.IsPrerecorded && request.File is not null;
        var replaceMix = slot.Prerecord.IsPrerecorded && request.ReplaceMedia;
        var deleteMix = slot.Prerecord.IsPrerecorded && request.DeleteMedia;

        var primaryResult = DomainResult.Compose(slot.ChangePerformer(request.PerformerId),
                                                 slot.ChangeSubtitle(request.Subtitle),
                                                 slot.ChangeTime(request.StartsAt, request.Duration));
        
        this.ThrowIfDomainError(primaryResult);
        
        if (addMix)
        {
            var mixResult = await mixHandler.AddNewMixFile(this, schedule, request, ct);
            this.ThrowIfDomainError(mixResult);
            this.ThrowIfDomainError(slot.ReplacePrerecordedMix(mixResult.Value.UploadedFileName, 
                                                               mixResult.Value.AzuraCastMediaId));
        } 
        else if (replaceMix)
        {
            var mixResult = await mixHandler.ReplaceMixFile(this, slot, request, ct);
            this.ThrowIfDomainError(mixResult);
            this.ThrowIfDomainError(slot.ReplacePrerecordedMix(mixResult.Value.UploadedFileName, 
                                                               mixResult.Value.AzuraCastMediaId));
        }
        else if (deleteMix)
        {
            var mixResult = await mixHandler.DeleteMixFile(this, slot);
            this.ThrowIfDomainError(mixResult);
            this.ThrowIfDomainError(slot.DeletePrerecordedMix());
        }
        
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}