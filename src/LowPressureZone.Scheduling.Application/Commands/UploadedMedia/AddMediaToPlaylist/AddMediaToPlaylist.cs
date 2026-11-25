using FastEndpoints;
using LowPressureZone.Core;

namespace LowPressureZone.Scheduling.Application.Commands.UploadedMedia.AddMediaToPlaylist;

public record AddMediaToPlaylist(Guid Id) : ICommand<Result<string>>;