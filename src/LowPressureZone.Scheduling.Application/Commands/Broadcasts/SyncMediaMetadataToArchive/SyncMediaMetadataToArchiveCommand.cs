using FastEndpoints;
using LowPressureZone.Core;

namespace LowPressureZone.Scheduling.Application.Commands.Broadcasts.SyncMediaMetadataToArchive;

public record SyncMediaMetadataToArchiveCommand(Guid Id) : ICommand<Result<string>>;