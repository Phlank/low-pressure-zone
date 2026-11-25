using LowPressureZone.Core.Domain;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Events;

public record UploadedMediaCreated(Guid Id) : IEvent;