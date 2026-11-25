using LowPressureZone.Core.Domain.Rules;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class UploadedMediaPathRequiredRule(UploadedMedia value)
    : RequiredStringRule(nameof(UploadedMedia.FilePath), value.FilePath);