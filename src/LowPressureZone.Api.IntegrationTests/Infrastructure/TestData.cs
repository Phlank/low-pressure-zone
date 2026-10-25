using System.Net;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Core;
using LowPressureZone.Domain.BroadcastAggregate;

namespace LowPressureZone.Api.IntegrationTests.Infrastructure;

/// <summary>
///     Builders for AzuraCast API responses and domain data used by the tests.
/// </summary>
public static class TestData
{
    public static Result<T, HttpResponseMessage> Ok<T>(T value) => new(value, null);

    public static Result<T, HttpResponseMessage> Error<T>(HttpStatusCode statusCode)
        => new(default, new HttpResponseMessage(statusCode));

    public static Task<Result<IReadOnlyCollection<StationStreamerBroadcast>, HttpResponseMessage>> BroadcastsResult(
        params StationStreamerBroadcast[] broadcasts)
        => Task.FromResult(Ok<IReadOnlyCollection<StationStreamerBroadcast>>(broadcasts));

    public static StationStreamerBroadcast RemoteBroadcast(
        int id,
        int streamerId,
        string displayName,
        DateTime start,
        DateTime? end = null,
        bool hasRecording = true)
        => new()
        {
            Id = id,
            TimestampStart = start,
            TimestampEnd = end,
            Streamer = new StationStreamerBroadcastStreamer
            {
                Id = streamerId,
                StreamerUsername = displayName.ToLowerInvariant().Replace(' ', '_'),
                DisplayName = displayName
            },
            Recording = hasRecording
                            ? new StationStreamerBroadcastRecording
                            {
                                Path = $"recordings/{id}.mp3",
                                DownloadUrl = $"https://azuracast.example.com/recordings/{id}.mp3",
                                Size = 1024
                            }
                            : null
        };

    public static Broadcast LocalBroadcast(
        int id,
        int streamerId,
        string displayName,
        DateTimeOffset start,
        DateTimeOffset? end = null,
        bool hasFile = true,
        bool isArchived = false)
    {
        var broadcast = Broadcast.Create(id, streamerId, displayName, hasFile, start, end).Value;
        if (isArchived)
            broadcast.Archive().IsSuccess.ShouldBeTrue();

        return broadcast;
    }
}
