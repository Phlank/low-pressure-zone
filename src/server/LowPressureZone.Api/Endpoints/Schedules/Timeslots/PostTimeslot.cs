using FastEndpoints;
using FFMpegCore;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public partial class PostTimeslot(
    DataContext dataContext,
    FormFileSaver fileSaver,
    MediaAnalyzer mediaAnalyzer,
    PerformerRules performerRules,
    ScheduleRules scheduleRules)
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

        if (request.PerformanceType == PerformanceTypes.Prerecorded
            && request.File is not null)
        {
            var saveResult = await fileSaver.SaveFormFileAsync(request.File, ct);
            if (saveResult.IsError)
                ThrowError(nameof(request.File), "Unable to save file for analysis.");

            var analysisResult = await mediaAnalyzer.AnalyzeAsync(saveResult.Value, ct);
            if (analysisResult.IsError)
                ThrowError(nameof(request.File), "Unable to analyze media file: " + analysisResult.Error);

            ValidationFailures.AddRange(TimeslotRequestValidator.ValidateMediaAnalysis(request, analysisResult.Value));
            ThrowIfAnyErrors();
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

    [LoggerMessage(LogLevel.Error, "Unable to save file for analysis: {error}")]
    static partial void LogUnableToSaveFileError(ILogger logger, string error);
}