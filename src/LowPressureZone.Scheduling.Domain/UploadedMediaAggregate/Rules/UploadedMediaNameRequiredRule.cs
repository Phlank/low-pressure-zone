using LowPressureZone.Core.Domain.Rules;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Rules;

public class UploadedMediaNameRequiredRule(UploadedMedia value)
    : RequiredStringRule(nameof(UploadedMedia.FileName), value.FileName);