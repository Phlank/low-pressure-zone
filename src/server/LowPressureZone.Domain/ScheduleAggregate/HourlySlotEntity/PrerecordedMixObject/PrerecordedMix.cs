using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject.Rules;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject;

public readonly record struct PrerecordedMix
{
    [MaxLength(2048)] public string? UploadedFileName { get; private init; }
    public int? AzuraCastMediaId { get; private init; }

    public static DomainResult<PrerecordedMix> Create(string? uploadedFileName, int? azuraCastMediaId) =>
        Rule.ApplyIntoResult(new PrerecordedMix
                             {
                                 UploadedFileName = uploadedFileName?.Trim(),
                                 AzuraCastMediaId = azuraCastMediaId
                             },
                             new FileNameLengthCannotExceed2048Rule(uploadedFileName),
                             new NameRequiredWhenMediaIdProvidedRule(uploadedFileName, azuraCastMediaId),
                             new MediaIdRequiredWhenNameProvidedRule(uploadedFileName, azuraCastMediaId));
}