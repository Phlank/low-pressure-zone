using FastEndpoints;
using LowPressureZone.Core;
using IEvent = LowPressureZone.Core.Domain.IEvent;

namespace LowPressureZone.Scheduling.Domain.UploadedMediaAggregate.Events;

public record UploadedMediaLinkedToPlaylist(Guid Id) : IEvent, ICommand<Result<string>>;