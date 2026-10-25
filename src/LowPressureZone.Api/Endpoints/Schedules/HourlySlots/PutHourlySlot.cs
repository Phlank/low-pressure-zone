using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class PutHourlySlot(
    DataContext dataContext,
    HourlySlotPermissions permissions,
    PerformerPermissions performerPermissions,
    HourlySlotPrerecordedMixHandler mixHandler) : Endpoint<HourlySlotRequest>
{
    public override void Configure()
    {
        Put("/schedules/{scheduleId}/hourlySlots/{id}");
        AllowFormData();
        AllowFileUploads();
        Description(b => b.Produces(201));
        Roles(RoleNames.AllRoles);
    }

    public override async Task HandleAsync(HourlySlotRequest request, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var id = Route<Guid>("id");

        var schedule = await dataContext.Schedules
                                        .Include(schedule => schedule.Community)
                                        .ThenInclude(community => community.Relationships
                                                                           .Where(relationship =>
                                                                                      relationship.UserId ==
                                                                                      User.GetIdOrDefault()))
                                        .Where(schedule => schedule.Id == scheduleId)
                                        .FirstOrDefaultAsync(ct);
        
        var slot = schedule?.HourlySlots.FirstOrDefault(slot => slot.Id == id);

        if (schedule is null || slot is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!permissions.IsEditAuthorized(slot))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var performer = await dataContext.Performers.FirstOrDefaultAsync(p => p.Id == request.PerformerId, ct);
        if (performer is null || !performerPermissions.IsHourlySlotLinkAuthorized(performer))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var addMix = !slot.Prerecord.IsPrerecorded && request.File is not null;
        var replaceMix = slot.Prerecord.IsPrerecorded && request.ReplaceMedia;
        var deleteMix = slot.Prerecord.IsPrerecorded && request.DeleteMedia;

        var primaryResult = DomainResult.Compose(slot.ChangePerformer(request.PerformerId),
                                                 slot.ChangeSubtitle(request.Subtitle),
                                                 slot.ChangeTime(request.StartsAt, request.Duration));
        
        await this.PublishOrThrowAsync(primaryResult);
        
        if (addMix)
        {
            var mixResult = await mixHandler.AddNewMixFile(this, schedule, request, ct);
            await this.PublishOrThrowAsync(mixResult);
            await this.PublishOrThrowAsync(slot.ReplacePrerecordedMix(mixResult.Value.UploadedFileName, 
                                                               mixResult.Value.AzuraCastMediaId));
        } 
        else if (replaceMix)
        {
            var mixResult = await mixHandler.ReplaceMixFile(this, slot, request, ct);
            await this.PublishOrThrowAsync(mixResult);
            await this.PublishOrThrowAsync(slot.ReplacePrerecordedMix(mixResult.Value.UploadedFileName, 
                                                               mixResult.Value.AzuraCastMediaId));
        }
        else if (deleteMix)
        {
            var mixResult = await mixHandler.DeleteMixFile(this, slot);
            await this.PublishOrThrowAsync(mixResult);
            await this.PublishOrThrowAsync(slot.DeletePrerecordedMix());
        }
        
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}