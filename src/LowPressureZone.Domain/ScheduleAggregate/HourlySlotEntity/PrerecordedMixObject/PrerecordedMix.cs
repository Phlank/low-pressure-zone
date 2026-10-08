using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject.Rules;

namespace LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject;

public readonly record struct PrerecordedMix
{
    public bool IsPrerecorded { get; private init; }

    [MaxLength(2048)]
    public string? UploadedFileName { get; private init; }

    public int? AzuraCastMediaId { get; private init; }

    public static DomainResult<PrerecordedMix> Create(bool isPrerecorded,
                                                      string? uploadedFileName,
                                                      int? azuraCastMediaId) =>
        Rule.ApplyIntoResult(new PrerecordedMix
                                          {
                                              IsPrerecorded = isPrerecorded,
                                              UploadedFileName = uploadedFileName?.Trim(),
                                              AzuraCastMediaId = azuraCastMediaId
                                          },
                                          new FileNameRequiredWhenPrerecordedRule(isPrerecorded, uploadedFileName),
                                          new MediaRequiredWhenPrerecordedRule(isPrerecorded, azuraCastMediaId),
                                          new FileNameLengthCannotExceed2048Rule(uploadedFileName));

    public static DomainResult<PrerecordedMix> Cleanup =>
        Rule.ApplyIntoResult(new PrerecordedMix
                                          {
                                              IsPrerecorded = false,
                                              UploadedFileName = null,
                                              AzuraCastMediaId = null
                                          },
                                          new FileNameRequiredWhenPrerecordedRule(false, null),
                                          new MediaRequiredWhenPrerecordedRule(false, null),
                                          new FileNameLengthCannotExceed2048Rule(null));
}