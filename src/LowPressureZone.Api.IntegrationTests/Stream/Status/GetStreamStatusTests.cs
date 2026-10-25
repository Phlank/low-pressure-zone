using System.Net;
using LowPressureZone.Api.Endpoints.Stream.Status;
using LowPressureZone.Api.IntegrationTests.Infrastructure;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.IntegrationTests.Stream.Status;

public sealed class GetStreamStatusTests(ApiFixture app) : ApiTestBase(app)
{
    private static HttpRequestMessage Request()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/stream/status?test={Guid.NewGuid():N}");
        request.Headers.CacheControl = new() { NoCache = true };
        return request;
    }

    [Fact]
    public async Task Anonymous_user_can_read_stream_status()
    {
        App.StreamStatus.Status = new StreamStatus
        {
            IsOnline = true,
            IsLive = true,
            IsStatusStale = false,
            Name = "DJ Test",
            Type = "AzuraCast",
            ListenerCount = 12,
            StartedAt = new DateTimeOffset(2026, 10, 25, 10, 0, 0, TimeSpan.Zero),
            Duration = TimeSpan.FromMinutes(7),
            Mounts = [new Mount("Main", "https://stream.example.test/main")]
        };

        var response = await App.CreateAnonymousClient().SendAsync(Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<StreamStatusResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.IsOnline.ShouldBeTrue();
        body.IsLive.ShouldBeTrue();
        body.IsStatusStale.ShouldBeFalse();
        body.Name.ShouldBe("DJ Test");
        body.Type.ShouldBe("AzuraCast");
        body.ListenerCount.ShouldBe(12);
        body.StartedAt.ShouldBe(App.StreamStatus.Status.StartedAt);
        body.DurationSeconds.ShouldBe(420);
        body.Mounts.ShouldBe([new Mount("Main", "https://stream.example.test/main")]);
        App.StreamStatus.StartCount.ShouldBe(1);
    }

    [Fact]
    public async Task Unavailable_status_returns_conflict()
    {
        var response = await App.CreateAnonymousClient().SendAsync(Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldContain("AzuraCast server is unavailable.");
        App.StreamStatus.StartCount.ShouldBe(1);
    }

    [Fact]
    public async Task Already_started_status_service_is_not_started_again()
    {
        App.StreamStatus.Status = new StreamStatus
        {
            IsOnline = false,
            IsLive = false,
            IsStatusStale = true,
            Name = null,
            Type = null,
            ListenerCount = 0,
            StartedAt = null,
            Duration = null,
            Mounts = []
        };
        await App.StreamStatus.StartAsync(TestContext.Current.CancellationToken);

        var response = await App.CreateAnonymousClient().SendAsync(Request(), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        App.StreamStatus.StartCount.ShouldBe(1);
        var body = await response.Content.ReadFromJsonAsync<StreamStatusResponse>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.IsStatusStale.ShouldBeTrue();
        body.Name.ShouldBeNull();
        body.StartedAt.ShouldBeNull();
        body.DurationSeconds.ShouldBeNull();
        body.Mounts.ShouldBeEmpty();
    }
}
