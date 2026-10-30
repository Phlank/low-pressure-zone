using FluentEmail.Core;
using FluentEmail.Core.Models;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Api.Services.NightlyTasks;
using LowPressureZone.Api.Services.StreamStatus;
using LowPressureZone.Data;
using LowPressureZone.Scheduling.Domain.BroadcastAggregate;
using LowPressureZone.Scheduling.Domain.CommunityAggregate;
using LowPressureZone.Scheduling.Domain.CommunityAggregate.RelationshipEntity;
using LowPressureZone.Scheduling.Domain.PerformerAggregate;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.ClashSlotEntity;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using LowPressureZone.Infrastructure.Configuration.Streaming;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace LowPressureZone.Api.IntegrationTests.Infrastructure;

using NewsItem = LowPressureZone.Domain.NewsAggregate.News;

/// <summary>
///     Hosts the API in memory with SQLite in-memory databases, a faked AzuraCast client, an in-memory Quartz scheduler,
///     and header based test authentication. No background services are started.
/// </summary>
public sealed class ApiFixture : AppFixture<Program>
{
    public IAzuraCastClient AzuraCast => Services.GetRequiredService<IAzuraCastClient>();

    public TestStreamStatusService StreamStatus => Services.GetRequiredService<TestStreamStatusService>();

    public ValueTask<IScheduler> Scheduler => Services.GetRequiredService<ISchedulerFactory>().GetScheduler();

    protected override void ConfigureApp(IWebHostBuilder a) => a.UseEnvironment("Testing");

    protected override void ConfigureServices(IServiceCollection s)
    {
        RemoveBackgroundServices(s);
        UseSqliteInMemory<DataContext>(s);
        UseSqliteInMemory<IdentityContext>(s);

        s.RemoveAll<IAzuraCastClient>();
        s.AddSingleton(A.Fake<IAzuraCastClient>());

        var emailFactory = A.Fake<IFluentEmailFactory>();
        var email = A.Fake<IFluentEmail>();
        A.CallTo(() => email.SetFrom(A<string>._, A<string>._)).Returns(email);
        A.CallTo(() => email.To(A<string>._, A<string>._)).Returns(email);
        A.CallTo(() => email.Subject(A<string>._)).Returns(email);
        A.CallTo(() => email.Body(A<string>._, A<bool>._)).Returns(email);
        A.CallTo(() => email.SendAsync()).Returns(new SendResponse());
        A.CallTo(() => emailFactory.Create()).Returns(email);
        s.RemoveAll<IFluentEmailFactory>();
        s.AddSingleton(emailFactory);

        s.RemoveAll<IStreamStatusService>();
        s.AddSingleton<TestStreamStatusService>();
        s.AddSingleton<IStreamStatusService>(provider => provider.GetRequiredService<TestStreamStatusService>());
        s.PostConfigure<StreamingConfiguration>(configuration =>
        {
            configuration.Live = new AzuraCastStreamConfiguration
            {
                Host = "live.example.test",
                Port = "8000",
                Mount = "/radio.mp3"
            };
            configuration.Test = new IcecastStreamConfiguration
            {
                Host = "test.example.test",
                Port = "8001",
                Mount = "/test.mp3",
                Username = "test-user",
                Password = "test-password"
            };
        });

        s.AddAuthentication(options =>
         {
             options.DefaultScheme = TestAuthHandler.SchemeName;
             options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
             options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
             options.DefaultForbidScheme = TestAuthHandler.SchemeName;
         })
         .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, null);
    }

    protected override async ValueTask SetupAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DataContext>().Database.EnsureCreatedAsync();
        await scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.EnsureCreatedAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        foreach (var roleName in RoleNames.DatabaseRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = roleName });
        }
    }

    /// <summary>
    ///     Clears all per-test state: database rows, scheduled Quartz jobs, and the AzuraCast fake.
    /// </summary>
    public async Task ResetAsync()
    {
        FakeItEasy.Fake.Reset(AzuraCast);
        StreamStatus.Reset();

        await (await Scheduler).Clear();

        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dataContext.Broadcasts.ExecuteDeleteAsync();
        await dataContext.News.ExecuteDeleteAsync();
        await dataContext.ClashSlots.ExecuteDeleteAsync();
        await dataContext.HourlySlots.ExecuteDeleteAsync();
        await dataContext.Schedules.ExecuteDeleteAsync();
        await dataContext.Performers.ExecuteDeleteAsync();
        await dataContext.Set<Relationship>().ExecuteDeleteAsync();
        await dataContext.Communities.ExecuteDeleteAsync();

        var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await identityContext.UserRoles.ExecuteDeleteAsync();
        await identityContext.Users.ExecuteDeleteAsync();
    }

    public async Task<AppUser> CreateUserAsync(string userName, int? streamerId = null, bool isAdmin = false)
    {
        await using var scope = Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = $"{userName}@example.com",
            EmailConfirmed = true,
            DisplayName = userName,
            StreamerId = streamerId
        };
        (await userManager.CreateAsync(user)).Succeeded.ShouldBeTrue();
        if (isAdmin)
            (await userManager.AddToRoleAsync(user, RoleNames.Admin)).Succeeded.ShouldBeTrue();

        return user;
    }

    /// <summary>
    ///     Organizer is not an Identity role; it is derived from community relationships by the claims transformation.
    /// </summary>
    public Task<Community> MakeOrganizerAsync(AppUser user)
        => AddCommunityAsync($"Community {Guid.NewGuid():N}",
                             configure: community => community.SetRolesForUser(user.Id, false, true));

    public async Task<Community> AddCommunityAsync(
        string name,
        string? socialUrl = null,
        Action<Community>? configure = null)
    {
        var community = Community.Create(name, socialUrl ?? $"https://example.com/{Guid.NewGuid():N}").Value;
        configure?.Invoke(community);

        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Communities.Add(community);
        await dataContext.SaveChangesAsync();
        return community;
    }

    public async Task<List<Community>> GetCommunitiesAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<DataContext>()
                          .Communities.AsNoTracking()
                          .Include(community => community.Relationships)
                          .ToListAsync();
    }

    /// <summary>
    ///     <see cref="NewsItem.PublishedAt" /> is init-only and defaults to now, so it is overridden through EF when given.
    /// </summary>
    public async Task<NewsItem> AddNewsAsync(string title, string content = "Content", DateTimeOffset? publishedAt = null)
    {
        var news = NewsItem.Create(title, content).Value;

        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.News.Add(news);
        if (publishedAt is not null)
            dataContext.Entry(news).Property(n => n.PublishedAt).CurrentValue = publishedAt.Value;
        await dataContext.SaveChangesAsync();
        return news;
    }

    public async Task<List<NewsItem>> GetNewsAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<DataContext>().News.AsNoTracking().ToListAsync();
    }

    public async Task<Performer> AddPerformerAsync(
        Guid creatorUserId,
        string name,
        string? socialUrl = null,
        bool isDeleted = false)
    {
        var performer = Performer.Create(creatorUserId, name, socialUrl).Value;
        if (isDeleted)
            performer.Delete();

        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Performers.Add(performer);
        await dataContext.SaveChangesAsync();
        return performer;
    }

    public async Task<List<Performer>> GetPerformersAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<DataContext>().Performers.AsNoTracking().ToListAsync();
    }

    public async Task<Schedule> AddScheduleAsync(
        Guid communityId,
        string name = "Schedule",
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        bool isHourlyAllowed = true,
        bool isClashAllowed = true,
        bool isVisibleToPublic = false)
    {
        var start = startsAt ?? DateTimeOffset.UtcNow.AddDays(10);
        var end = endsAt ?? start.AddHours(8);
        var schedule = Schedule.Create(name, "Description", communityId, start, end,
                                       isHourlyAllowed, isClashAllowed, isVisibleToPublic).Value;

        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Schedules.Add(schedule);
        await dataContext.SaveChangesAsync();
        return schedule;
    }

    public async Task<HourlySlot> AddHourlySlotAsync(
        Guid scheduleId,
        Guid performerId,
        DateTimeOffset startsAt,
        int duration = 1,
        string? subtitle = null)
    {
        var slot = HourlySlot.Create(scheduleId, performerId, startsAt, duration, subtitle).Value;

        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Set<HourlySlot>().Add(slot);
        await dataContext.SaveChangesAsync();
        return slot;
    }

    public async Task<ClashSlot> AddClashSlotAsync(
        Guid scheduleId,
        Guid performerOneId,
        Guid performerTwoId,
        DateTimeOffset startsAt,
        int duration = 2,
        params string[] rounds)
    {
        var slot = ClashSlot.Create(scheduleId, performerOneId, performerTwoId, [.. rounds], startsAt, duration).Value;

        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Set<ClashSlot>().Add(slot);
        await dataContext.SaveChangesAsync();
        return slot;
    }

    public async Task<List<Schedule>> GetSchedulesAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<DataContext>().Schedules.AsNoTracking().ToListAsync();
    }

    public async Task AddBroadcastsAsync(params Broadcast[] broadcasts)
    {
        await using var scope = Services.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Broadcasts.AddRange(broadcasts);
        await dataContext.SaveChangesAsync();
    }

    public async Task<List<Broadcast>> GetBroadcastsAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<DataContext>()
                          .Broadcasts.AsNoTracking()
                          .ToListAsync();
    }

    /// <summary>
    ///     Endpoints are throttled per client IP, which the test server does not provide, so every client sends a unique
    ///     <c>X-Forwarded-For</c> value per request.
    /// </summary>
    private static ClientOptions ThrottleBypassOptions => new() { ThrottleBypassHeaderName = "X-Forwarded-For" };

    public HttpClient CreateAnonymousClient() => CreateClient(ThrottleBypassOptions);

    public HttpClient CreateClientFor(AppUser user)
        => CreateClient(client => client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, user.Id.ToString()),
                        ThrottleBypassOptions);

    private static void RemoveBackgroundServices(IServiceCollection services)
    {
        var toRemove = services.Where(descriptor =>
                                          descriptor.ServiceType == typeof(IHostedService)
                                          && !descriptor.IsKeyedService
                                          && (descriptor.ImplementationType == typeof(BroadcastSyncService)
                                              || descriptor.ImplementationType == typeof(NightlyTaskService)))
                               .ToList();
        foreach (var descriptor in toRemove)
            services.Remove(descriptor);
    }

    private static void UseSqliteInMemory<TContext>(IServiceCollection services) where TContext : DbContext
    {
        services.RemoveAll<DbContextOptions<TContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<TContext>>();

        // Each in-memory SQLite database lives only as long as its connection, so the connection is a singleton
        // owned (and disposed) by the test host.
        var connectionKey = typeof(TContext).Name;
        services.AddKeyedSingleton<SqliteConnection>(connectionKey, (_, _) =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        });
        services.AddDbContext<TContext>((provider, options) =>
        {
            options.UseSqlite(provider.GetRequiredKeyedService<SqliteConnection>(connectionKey))
                   .ReplaceService<IModelCustomizer, SqliteModelCustomizer>();
        });
    }
}
